using Helpers;
using Helpers.Assets.Scripts.Helpers;

using Input.Components;

using Maps.Authoring;
using Maps.Components;

using NUnit.Framework;

using Scripts.Assets.Scripts.Input.Systems;

using Unity.Entities;
using Unity.Mathematics;

using static NUnit.Framework.Assert;

namespace Tests.Input
{
	public class MouseOverTileTest : ECSSystemTester<MouseOverTileSystem>
	{
		private TileTypeSO SandTileSO;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			SandTileSO = ResourcePath.TILES_SO.Load<TileTypeSO>( "Sand" );
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			SandTileSO = null;
		}

		[Test]
		public void NoMap_NoTile()
		{
			var entity = SetMouseInput( float2.zero );

			Update();

			var tileUnderMouse = _entityManager.GetComponentData<TileUnderMouse>( entity );
			AreEqual( new Entity(), tileUnderMouse.Tile );
			False( _entityManager.Exists( tileUnderMouse.Tile ) );
		}

		[Test]
		public void OneTile_Result()
		{
			var entity = SetMouseInput( float2.zero );

			SetMap( 1 );

			Update();

			var tileUnderMouse = _entityManager.GetComponentData<TileUnderMouse>( entity );
			True( _entityManager.Exists( tileUnderMouse.Tile ) );
		}

		[Test]
		public void OneTile_IsSand()
		{
			var entity = SetMouseInput( float2.zero );

			SetMap( 1 );

			Update();

			var tileUnderMouse = _entityManager.GetComponentData<TileUnderMouse>( entity );
			AreEqual( SandTileSO.name, _entityManager.GetSharedComponentData<TileType>( tileUnderMouse.Tile ).TileTypeBlob.Value.Name.ToString() );
		}

		[Test]
		public void FiveTileEdge_Result()
		{
			var entity = SetMouseInput( float2.zero );

			SetMap( 5 );

			Update();

			var tileUnderMouse = _entityManager.GetComponentData<TileUnderMouse>( entity );
			True( _entityManager.Exists( tileUnderMouse.Tile ) );
		}

		[Test]
		public void FiveTileEdge_IsSand()
		{
			var entity = SetMouseInput( float2.zero );

			SetMap( 5 );

			Update();

			var tileUnderMouse = _entityManager.GetComponentData<TileUnderMouse>( entity );
			AreEqual( SandTileSO.name, _entityManager.GetSharedComponentData<TileType>( tileUnderMouse.Tile ).TileTypeBlob.Value.Name.ToString() );
		}

		protected Entity SetMouseInput( float2 position )
		{
			var mousePosition = new MouseWorldPosition(){ Position = position, Delta = float2.zero };
			var tileUnderMouse = new TileUnderMouse();

			var entity = _entityManager.CreateEntity( mousePosition.GetType(), tileUnderMouse.GetType() );

			_entityManager.SetComponentData( entity, mousePosition );
			_entityManager.SetComponentData( entity, tileUnderMouse );

			return entity;
		}

		protected Entity SetMouseInput( float2 position, float2 delta )
		{
			var mousePosition = new MouseWorldPosition(){ Position = position, Delta = delta };
			var tileUnderMouse = new TileUnderMouse();

			var entity = _entityManager.CreateEntity( mousePosition.GetType(), tileUnderMouse.GetType() );

			_entityManager.SetComponentData( entity, mousePosition );
			_entityManager.SetComponentData( entity, tileUnderMouse );

			return entity;
		}

		private void SetMap( int edgeSize )
		{
			var mapSize = edgeSize * edgeSize;
			var tile = _entityManager.CreateEntity( typeof(TileType) );
			_entityManager.SetSharedComponentData( tile, new TileType( SandTileSO ) );
			var tiles = new BlitableArray<Entity>(mapSize, Unity.Collections.Allocator.Temp);
			for ( int i = 0; i < mapSize; i++ )
			{
				tiles[i] = tile;
			}
			_entityManager.SetComponentData( _entityManager.CreateEntity( typeof( MapSettings ) ), new MapSettings() { Tiles = tiles, MapEdgeSize = 1 } );
		}
	}
}