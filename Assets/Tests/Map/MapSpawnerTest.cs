using Helpers;

using Maps.Authoring;
using Maps.Components;
using Maps.Systems;

using NUnit.Framework;

using System.Linq;

using Unity.Collections;
using Unity.Mathematics;

using UnityEngine;

using static NUnit.Framework.Assert;

namespace Tests.Map
{
	public class MapSpawnerTest : ECSSystemTester<MapSpawner>
	{
		private TileTypeSO SandTileSO;
		private TileTypeSO[] AllTileSO;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			AllTileSO = new TileTypeSO[10];
			for ( int i = 0; i < AllTileSO.Length; i++ )
			{
				AllTileSO[i] = ScriptableObject.CreateInstance<TileTypeSO>();
				AllTileSO[i].name = i.ToString();
				AllTileSO[i].Cost = i * i;
				AllTileSO[i].Color = new Color32( (byte)i, (byte)i, (byte)i, 1 );
				AllTileSO[i].Range = 1f / AllTileSO.Length;
				AllTileSO[i].hideFlags = HideFlags.HideAndDontSave;
				AllTileSO[i].SetupToString();
			}
			SandTileSO = AllTileSO.First();
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			AllTileSO = null;
			SandTileSO = null;
		}

		[Test]
		[Order( 1 )]
		public void Spawn_OneSand()
		{
			// Create request
			var requestEntity = _entityManager.CreateEntity(typeof(MapRequest));
			var tileTypes = new BlitableArray<TileType>( 1, Allocator.TempJob );
			tileTypes[0] = new TileType( SandTileSO );
			_entityManager.SetSharedComponentData( requestEntity, new MapRequest() { Frequency = new float2( 0.1f, 0.1f ), TileTypes = tileTypes, MapEdgeSize = 1 } );

			// Update
			Update();

			// Gather data
			var mapSettingEntities = _entityManager.CreateEntityQuery( typeof( MapSettings ) ).ToEntityArray(Allocator.TempJob);
			AreEqual( 1, mapSettingEntities.Length );

			var mapSetting = _entityManager.GetComponentData<MapSettings>(mapSettingEntities[0]);
			var created = mapSetting.Tiles;
			var tileType = _entityManager.GetSharedComponentData<TileType>( created[0] );

			AreEqual( 1, created.Length );
			AreEqual( tileType.TileTypeBlob.Value.Name.ToString(), SandTileSO.name );
			mapSettingEntities.Dispose();
		}

		[Test]
		[Order( 1 )]
		public void Spawn_BigRandom()
		{
			var mapSize = 50;

			// Create request
			var requestEntity = _entityManager.CreateEntity(typeof(MapRequest));
			var tileTypes = new BlitableArray<TileType>( AllTileSO.Length, Allocator.TempJob );

			for ( int i = 0; i < AllTileSO.Length; i++ )
			{
				tileTypes[i] = new TileType( AllTileSO[i] );
			}

			_entityManager.SetSharedComponentData( requestEntity, new MapRequest() { Frequency = new float2( 0.1f, 0.1f ), TileTypes = tileTypes, MapEdgeSize = mapSize } );

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