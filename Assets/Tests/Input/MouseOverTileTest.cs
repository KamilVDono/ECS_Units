using Blobs;

using Helpers;

using Input.Components;
using Input.Systems;

using Maps.Authoring;
using Maps.Components;

using NUnit.Framework;

using Tests.Utility;

using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

using static NUnit.Framework.Assert;

namespace Tests.Input
{
	public class MouseOverTileTest : ECSSystemTester<MouseOverTileSystem>
	{
		private string _sandName = "sand";

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			TileTypeSO SandTileSO = ScriptableObject.CreateInstance<TileTypeSO>();
			SandTileSO.name = _sandName;
			SandTileSO.Cost = 1f;
			SandTileSO.Color = Color.red;
			SandTileSO.Range = 1f;
			SandTileSO.hideFlags = HideFlags.HideAndDontSave;

			BlobsMemory.FromSOs( new[] { SandTileSO } );
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			BlobsMemory.Instance.Dispose();
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
			AreEqual( _sandName, _entityManager.GetComponentData<GroundType>( tileUnderMouse.Tile ).TileTypeBlob.Value.Name.ToString() );
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
			AreEqual( _sandName, _entityManager.GetComponentData<GroundType>( tileUnderMouse.Tile ).TileTypeBlob.Value.Name.ToString() );
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
			var tile = _entityManager.CreateEntity( typeof(GroundType) );
			_entityManager.SetComponentData( tile, new GroundType( BlobsMemory.Instance.ReferencesOf<GroundTypeBlob>()[0] ) );
			var tiles = new BlitableArray<Entity>(mapSize, Unity.Collections.Allocator.Temp);
			for ( int i = 0; i < mapSize; i++ )
			{
				tiles[i] = tile;
			}

			var mapEntity = _entityManager.CreateEntity( typeof( MapSettings ) );
			_entityManager.SetSharedComponentData( mapEntity, new MapSettings() { Tiles = tiles, MapEdgeSize = 1 } );
			TargetSystem.MapSettingsEntity = mapEntity;
		}
	}
}