using Blobs;
using Blobs.Interfaces;

using Maps.Authoring;
using Maps.Components;
using Maps.Systems;

using NUnit.Framework;

using Resources.Authoring;
using Resources.Components;

using Tests.Categories;
using Tests.Utility;

using Unity.Entities;

using UnityEngine;

using Working.Components;
using Working.Systems;

namespace Tests.Working
{
	// System requires: MiningWork, WorkProgress, ResourceOre, MapIndex, MapSettings uses system EndSimulationEntityCommandBufferSystem

	public class MiningWorkTest : ECSSystemTester<MiningWorkSystem>
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			TileTypeSO sandTileSO = ScriptableObject.CreateInstance<TileTypeSO>();
			sandTileSO.name = "sand";
			sandTileSO.Cost = 1f;
			sandTileSO.Color = Color.red;
			sandTileSO.Range = 1f;
			sandTileSO.hideFlags = HideFlags.HideAndDontSave;

			ResourceTypeSO coal = ScriptableObject.CreateInstance<ResourceTypeSO>();
			coal.MovementCost = 2;
			coal.WorkRequired = 1f;
			coal.PiecesPerWork = 2;

			ResourceTypeSO coalHard = ScriptableObject.CreateInstance<ResourceTypeSO>();
			coalHard.MovementCost = 2;
			coalHard.WorkRequired = 4f;
			coalHard.PiecesPerWork = 2;

			ResourceTypeSO fake = ScriptableObject.CreateInstance<ResourceTypeSO>();

			BlobsMemory.FromSOs( new IBlobableSO[] { sandTileSO, coal, coalHard, fake } );

			var ese = _currentWorld.CreateSystem<EndSimulationEntityCommandBufferSystem>();
			Inject( ese, "_removeCmdBufferSystem" );
		}

		[Test]
		[ECSTest]
		public void MinigWork_CenterOre_NoMined()
		{
			CreateMap( 10, 1 );

			Update();

			var mapSettings = TargetSystem.GetSingleton<MapSettings>();
			Assert.IsTrue( _entityManager.HasComponent<MiningWork>( mapSettings.Tiles[4] ) );
			Assert.AreEqual( 10, _entityManager.GetComponentData<ResourceOre>( mapSettings.Tiles[4] ).Count );
		}

		[Test]
		[ECSTest]
		public void MinigWork_CenterOre_MinedOne()
		{
			CreateMap( 1, 0 );

			Update();

			var mapSettings = TargetSystem.GetSingleton<MapSettings>();
			Assert.IsFalse( _entityManager.HasComponent<MiningWork>( mapSettings.Tiles[4] ) );

			var query = _entityManager.CreateEntityQuery( ComponentType.ReadOnly<MinedOre>() );

			Assert.AreEqual( 1, query.CalculateEntityCount() );

			var minedOres = query.ToComponentDataArray<MinedOre>(Unity.Collections.Allocator.TempJob);
			var mined = minedOres[0];
			Assert.AreEqual( 1, mined.MinedCount );
			minedOres.Dispose();
		}

		[Test]
		[ECSTest]
		public void MinigWork_CenterOre_ZeroWorkRequired()
		{
			CreateMap( 1, 2 );

			Update();

			var mapSettings = TargetSystem.GetSingleton<MapSettings>();
			Assert.IsTrue( _entityManager.HasComponent<MiningWork>( mapSettings.Tiles[4] ) );
			Assert.AreEqual( 1, _entityManager.GetComponentData<ResourceOre>( mapSettings.Tiles[4] ).Count );
		}

		[Test]
		[ECSTest]
		public void MinigWork_CenterOre_StockChange()
		{
			MinigWork_CenterOre_MinedOne();

			Update();

			var query = _entityManager.CreateEntityQuery( ComponentType.ReadOnly<StockCountChange>() );

			Assert.AreEqual( 1, query.CalculateEntityCount() );

			var stockChanges = query.ToComponentDataArray<StockCountChange>(Unity.Collections.Allocator.TempJob);
			var stockChange = stockChanges[0];
			Assert.AreEqual( 1, stockChange.Count );
			stockChanges.Dispose();
		}

		private void CreateMap( int oreCount = 10, int resourceTypeIndex = 0 )
		{
			// Generate map [3x3] with ore on center Setup MapSettings and others

			var request = _entityManager.CreateEntity( typeof( MapRequest ) );
			_entityManager.SetComponentData( request, new MapRequest()
			{
				Frequency = new Unity.Mathematics.float2( 0.01f, 0.01f ),
				MapEdgeSize = 3,
				Offset = new Unity.Mathematics.float2( 0, 0 )
			} );

			var spawner = _currentWorld.GetOrCreateSystem<MapSpawner>();
			spawner.Update();

			var mapSettings = TargetSystem.GetSingleton<MapSettings>();
			Assert.AreEqual( 9, mapSettings.Tiles.Length );

			//Add MiningWork and WorkProgress
			var oreType = BlobsMemory.Instance.ReferencesOf<ResourceTypeBlob>()[resourceTypeIndex];

			_entityManager.AddComponentData( mapSettings.Tiles[4], new MiningWork() );
			_entityManager.AddComponentData( mapSettings.Tiles[4], new WorkProgress()
			{
				Progress = 1f
			} );

			// Make sure center tile contains ore
			_entityManager.SetComponentData( mapSettings.Tiles[4], new ResourceOre() { Capacity = oreCount, Count = oreCount, Type = oreType } );
		}
	}
}