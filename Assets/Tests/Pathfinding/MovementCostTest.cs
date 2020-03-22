using Blobs;
using Blobs.Interfaces;

using Maps.Authoring;
using Maps.Components;

using NUnit.Framework;

using Pathfinding.Components;
using Pathfinding.Systems;

using Resources.Authoring;
using Resources.Components;

using Tests.Categories;
using Tests.Utility;

using Unity.Entities;
using Unity.Jobs;

using UnityEngine;

namespace Tests.Pathfinding
{
	public class MovementCostTest : ECSSystemTester<MovementCostTrackerSystem>
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			TileTypeSO SandTileSO = ScriptableObject.CreateInstance<TileTypeSO>();
			SandTileSO.name = "sand";
			SandTileSO.Cost = 1f;
			SandTileSO.Color = Color.red;
			SandTileSO.Range = 1f;
			SandTileSO.hideFlags = HideFlags.HideAndDontSave;

			ResourceTypeSO coal = ScriptableObject.CreateInstance<ResourceTypeSO>();
			coal.MovementCost = 2;
			coal.UnitSize = 1f;

			BlobsMemory.FromSOs( new IBlobableSO[] { SandTileSO, coal } );

			var ese = _currentWorld.CreateSystem<EndSimulationEntityCommandBufferSystem>();
			Inject( ese, "_removeCmdBufferSystem" );
		}

		[Test]
		[Order( 1 )]
		[ECS_INVALID_Test( "Target system do not run. For some reason \"ShouldRunSystme\" returns false. " )]
		public void Initialize_Without_Ore()
		{
			var tileArchetype = _entityManager.CreateArchetype(typeof( MovementCost ), typeof( GroundType ), typeof( ResourceOre ));
			var tile = _entityManager.CreateEntity( tileArchetype );

			_entityManager.SetComponentData( tile, new GroundType( BlobsMemory.Instance.ReferencesOf<GroundTypeBlob>()[0] ) );
			_entityManager.SetComponentData( tile, ResourceOre.EMPTY_ORE );

			Update();
			Update();

			Assert.AreEqual( 1, _entityManager.GetComponentData<MovementCost>( tile ).Cost );
		}

		[Test]
		[Order( 2 )]
		[ECS_INVALID_Test( "Target system do not run. For some reason \"ShouldRunSystme\" returns false. " )]
		public void Initialize_With_Ore()
		{
			var tileArchetype = _entityManager.CreateArchetype(typeof( MovementCost ), typeof( GroundType ), typeof( ResourceOre ));
			var tile = _entityManager.CreateEntity( tileArchetype );

			_entityManager.SetComponentData( tile, new GroundType( BlobsMemory.Instance.ReferencesOf<GroundTypeBlob>()[0] ) );
			_entityManager.SetComponentData( tile, new ResourceOre() { Capacity = 10, Count = 10, Type = BlobsMemory.Instance.ReferencesOf<ResourceTypeBlob>()[0] } );

			Update();

			Assert.AreEqual( 3, _entityManager.GetComponentData<MovementCost>( tile ).Cost );
		}

		[Test]
		[Order( 3 )]
		[ECS_INVALID_Test( "Target system do not run. For some reason \"ShouldRunSystme\" returns false. " )]
		public void Initialize_With_Stock()
		{
			var tileArchetype = _entityManager.CreateArchetype(typeof( MovementCost ), typeof( GroundType ), typeof( ResourceOre ), typeof( Stock ));
			var tile = _entityManager.CreateEntity( tileArchetype );

			_entityManager.SetComponentData( tile, new GroundType( BlobsMemory.Instance.ReferencesOf<GroundTypeBlob>()[0] ) );
			_entityManager.SetComponentData( tile, new ResourceOre() { Capacity = 10, Count = 10, Type = BlobsMemory.Instance.ReferencesOf<ResourceTypeBlob>()[0] } );
			_entityManager.SetComponentData( tile, new Stock() { Capacity = 50, Count = 1, Type = BlobsMemory.Instance.ReferencesOf<ResourceTypeBlob>()[0] } );

			Update();

			var exceptedCost = BlobsMemory.Instance.ReferencesOf<GroundTypeBlob>()[0].Value.MoveCost
				+ BlobsMemory.Instance.ReferencesOf<ResourceTypeBlob>()[0].Value.MovementCost * 2;
			Assert.AreEqual( exceptedCost, _entityManager.GetComponentData<MovementCost>( tile ).Cost );
		}

		[Test]
		[Order( 4 )]
		[ECS_INVALID_Test( "Target system do not run. For some reason \"ShouldRunSystme\" returns false. " )]
		public void AfterAddOre_JobSystem()
		{
			_currentWorld.CreateSystem<ChangeResourceOreJob>();
			var tileArchetype = _entityManager.CreateArchetype(typeof( MovementCost ), typeof( GroundType ), typeof( ResourceOre ));
			var tile = _entityManager.CreateEntity( tileArchetype );

			_entityManager.SetComponentData( tile, new GroundType( BlobsMemory.Instance.ReferencesOf<GroundTypeBlob>()[0] ) );
			_entityManager.SetComponentData( tile, ResourceOre.EMPTY_ORE );

			// Double update because MovementCostTrackerSystem get call before ChangeResourceOreJob
			// So here MovementCostTrackerSystem calculate for ground and empty ore, after that ore
			// become not empty
			Update();
			// Here MovementCostTrackerSystem calculate for ground and fake coal ore
			Update();

			Assert.AreEqual( 3, _entityManager.GetComponentData<MovementCost>( tile ).Cost );
		}

		[Test]
		[Order( 5 )]
		[ECS_INVALID_Test( "Target system do not run. For some reason \"ShouldRunSystme\" returns false. " )]
		public void AfterAddOre_RegularSystem()
		{
			_currentWorld.CreateSystem<ChangeResourceOre>();
			var tileArchetype = _entityManager.CreateArchetype(typeof( MovementCost ), typeof( GroundType ), typeof( ResourceOre ));
			var tile = _entityManager.CreateEntity( tileArchetype );

			_entityManager.SetComponentData( tile, new GroundType( BlobsMemory.Instance.ReferencesOf<GroundTypeBlob>()[0] ) );
			_entityManager.SetComponentData( tile, ResourceOre.EMPTY_ORE );

			// Double update because MovementCostTrackerSystem get call before ChangeResourceOre So
			// here MovementCostTrackerSystem calculate for ground and empty ore, after that ore
			// become not empty
			Update();
			// Here MovementCostTrackerSystem calculate for ground and fake coal ore
			Update();

			Assert.AreEqual( 3, _entityManager.GetComponentData<MovementCost>( tile ).Cost );
		}

		[DisableAutoCreation]
		public class ChangeResourceOreJob : JobComponentSystem
		{
			protected override JobHandle OnUpdate( JobHandle inputDependencies )
			{
				return Entities
					.WithoutBurst()
					.ForEach( ( ref ResourceOre resourceOre ) =>
				{
					resourceOre.Type = BlobsMemory.Instance.ReferencesOf<ResourceTypeBlob>()[0];
					resourceOre.Capacity = 10;
					resourceOre.Count = 10;
				} ).Schedule( inputDependencies );
			}
		}

		[DisableAutoCreation]
		public class ChangeResourceOre : ComponentSystem
		{
			protected override void OnUpdate()
			{
				Entities
					.ForEach( ( ref ResourceOre resourceOre ) =>
				{
					resourceOre.Type = BlobsMemory.Instance.ReferencesOf<ResourceTypeBlob>()[0];
					resourceOre.Capacity = 10;
					resourceOre.Count = 10;
				} );
			}
		}
	}
}