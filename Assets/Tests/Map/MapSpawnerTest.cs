using Blobs;

using Helpers;

using Maps.Authoring;
using Maps.Components;
using Maps.Systems;

using NUnit.Framework;

using Unity.Collections;
using Unity.Mathematics;

using UnityEngine;

using static NUnit.Framework.Assert;

namespace Tests.Map
{
	public class MapSpawnerTest : ECSSystemTester<MapSpawner>
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			TileTypeSO[] AllTileSO = new TileTypeSO[10];
			for ( int i = 0; i < AllTileSO.Length; i++ )
			{
				AllTileSO[i] = ScriptableObject.CreateInstance<TileTypeSO>();
				AllTileSO[i].name = i.ToString();
				AllTileSO[i].Cost = i * i;
				AllTileSO[i].Color = new Color32( (byte)i, (byte)i, (byte)i, 1 );
				AllTileSO[i].Range = 1f / AllTileSO.Length;
				AllTileSO[i].hideFlags = HideFlags.HideAndDontSave;
			}

			BlobsMemory.FromSOs( AllTileSO );
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			BlobsMemory.Instance.Dispose();
		}

		[Test]
		[Order( 1 )]
		public void Spawn_OneSand()
		{
			// Create request
			var requestEntity = _entityManager.CreateEntity(typeof(MapRequest));
			var tileTypes = new BlitableArray<GroundType>( 1, Allocator.TempJob );
			_entityManager.SetSharedComponentData( requestEntity, new MapRequest() { Frequency = new float2( 0.1f, 0.1f ), MapEdgeSize = 1 } );

			// Update
			Update();

			// Gather data
			var mapSettingEntities = _entityManager.CreateEntityQuery( typeof( MapSettings ) ).ToEntityArray(Allocator.TempJob);
			AreEqual( 1, mapSettingEntities.Length );

			var mapSetting = _entityManager.GetComponentData<MapSettings>(mapSettingEntities[0]);
			var created = mapSetting.Tiles;

			AreEqual( 1, created.Length );
			mapSettingEntities.Dispose();
		}

		[Test]
		[Order( 1 )]
		public void Spawn_BigRandom()
		{
			var mapSize = 50;

			// Create request
			var requestEntity = _entityManager.CreateEntity(typeof(MapRequest));

			_entityManager.SetSharedComponentData( requestEntity, new MapRequest() { Frequency = new float2( 0.1f, 0.1f ), MapEdgeSize = mapSize } );

			// Update
			Update();

			// Gather data
			var mapSettingEntities = _entityManager.CreateEntityQuery( typeof( MapSettings ) ).ToEntityArray(Allocator.TempJob);
			AreEqual( 1, mapSettingEntities.Length );
			var created = _entityManager.GetComponentData<MapSettings>( mapSettingEntities[0] ).Tiles;

			AreEqual( mapSize * mapSize, created.Length );
			mapSettingEntities.Dispose();
		}
	}
}