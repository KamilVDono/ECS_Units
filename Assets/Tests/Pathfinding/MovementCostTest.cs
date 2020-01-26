using Blobs;
using Blobs.Interfaces;

using Maps.Authoring;
using Maps.Components;

using NUnit.Framework;

using Pathfinding.Components;
using Pathfinding.Systems;

using Resources.Authoring;
using Resources.Components;

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

			BlobsMemory.FromSOs( new IBlobableSO[] { SandTileSO, coal } );
		}

		[Test]
		public void Initialize_Without_Ore()
		{
			var tileArchetype = _entityManager.CreateArchetype(typeof( MovementCost ), typeof( GroundType ), typeof( ResourceOre ));
			var tile = _entityManager.CreateEntity( tileArchetype );

			_entityManager.SetComponentData( tile, new GroundType( BlobsMemory.Instance.ReferencesOf<GroundTypeBlob>()[0] ) );
			_entityManager.SetComponentData( tile, ResourceOre.EMPTY_ORE );

			Update();

			Assert.AreEqual( 1, _entityManager.GetComponentData<MovementCost>( tile ).Cost );
		}

		[Test]
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