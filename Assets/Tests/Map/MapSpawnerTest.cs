using Helpers;
using Helpers.Assets.Scripts.Helpers;

using Maps.Authoring;
using Maps.Components;
using Maps.Systems;

using NUnit.Framework;

using System.Linq;

using Unity.Collections;
using Unity.Mathematics;

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
			AllTileSO = ResourcePath.TILES_SO.All<TileTypeSO>();
			SandTileSO = AllTileSO.First( tile => tile.name == "Sand" );
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
			var requestEntity = _entityManager.CreateEntity(typeof(MapRequest), typeof(MapSettings));
			var tileTypes = new BlitableArray<TileType>( 1, Allocator.TempJob );
			tileTypes[0] = new TileType() { TileTypeSO = SandTileSO };
			_entityManager.SetSharedComponentData( requestEntity, new MapRequest() { Frequency = new float2( 0.1f, 0.1f ), TileTypes = tileTypes } );
			_entityManager.SetComponentData( requestEntity, new MapSettings() { CanMoveDiagonally = true, MapSize = 1 } );

			// Update
			Update();

			// Gather data
			var created = _entityManager.GetComponentData<MapSettings>( requestEntity ).Tiles;
			var tileType = _entityManager.GetSharedComponentData<TileType>( created[0] );

			AreEqual( 1, created.Length );
			AreEqual( tileType.TileTypeSO, SandTileSO );
		}

		[Test]
		public void Spawn_TwoTimes()
		{
			Spawn_OneSand();
			Spawn_OneSand();
		}

		[Test]
		[Order( 1 )]
		public void Spawn_BigRandom()
		{
			var mapSize = 50;

			// Create request
			var requestEntity = _entityManager.CreateEntity(typeof(MapRequest), typeof(MapSettings));
			var tileTypes = new BlitableArray<TileType>( AllTileSO.Length, Allocator.TempJob );

			for ( int i = 0; i < AllTileSO.Length; i++ )
			{
				tileTypes[i] = new TileType() { TileTypeSO = AllTileSO[i] };
			}

			_entityManager.SetSharedComponentData( requestEntity, new MapRequest() { Frequency = new float2( 0.1f, 0.1f ), TileTypes = tileTypes } );
			_entityManager.SetComponentData( requestEntity, new MapSettings() { CanMoveDiagonally = true, MapSize = mapSize } );

			// Update
			Update();

			// Gather data
			var created = _entityManager.GetComponentData<MapSettings>( requestEntity ).Tiles;

			AreEqual( mapSize * mapSize, created.Length );
		}
	}
}