using Helpers.Types;

using Maps.Components;

using NUnit.Framework;

using Pathfinding.Components;
using Pathfinding.Systems;

using System;
using System.Text;

using Tests.Utility;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

using static Helpers.IndexUtils;

namespace Tests.Pathfinding
{
	public class AStartTest : ECSSystemTester<AStar>
	{
		[Test]
		public void NoWay_Simple()
		{
			CreateMap( new float[] {
				MovementCost.IMPOSSIBLE, MovementCost.IMPOSSIBLE,
				MovementCost.IMPOSSIBLE, MovementCost.IMPOSSIBLE,
			} );

			var request = _entityManager.CreateEntity(typeof(PathRequest));
			_entityManager.SetComponentData( request, new PathRequest { Start = new int2( 0, 0 ), End = new int2( 1, 1 ) } );

			Update();

			var way = _entityManager.GetBuffer<Waypoint>( request );
			PrintWay( way );
			Assert.True( _entityManager.GetComponentData<PathRequest>( request ).Done );
			Assert.Zero( way.Length );
		}

		[Test]
		public void NoWay_Wall()
		{
			CreateMap( new float[] {
				MovementCost.FREE, MovementCost.FREE, MovementCost.FREE,
				MovementCost.IMPOSSIBLE, MovementCost.IMPOSSIBLE, MovementCost.IMPOSSIBLE,
				MovementCost.FREE, MovementCost.FREE, MovementCost.FREE,
			} );

			var request = _entityManager.CreateEntity(typeof(PathRequest));
			_entityManager.SetComponentData( request, new PathRequest { Start = new int2( 0, 0 ), End = new int2( 2, 2 ) } );

			Update();

			var way = _entityManager.GetBuffer<Waypoint>( request );
			PrintWay( way );
			Assert.True( _entityManager.GetComponentData<PathRequest>( request ).Done );
			Assert.Zero( way.Length );
		}

		[Test]
		public void From_IMPOSIBLE()
		{
			CreateMap( new float[] {
				MovementCost.IMPOSSIBLE, MovementCost.FREE,
				MovementCost.FREE, MovementCost.FREE,
			} );

			var request = _entityManager.CreateEntity(typeof(PathRequest));
			_entityManager.SetComponentData( request, new PathRequest { Start = new int2( 0, 0 ), End = new int2( 1, 1 ) } );

			Update();

			var way = _entityManager.GetBuffer<Waypoint>( request );
			PrintWay( way );
			Assert.True( _entityManager.GetComponentData<PathRequest>( request ).Done );

			var expectedPath = new int2[] { new int2( 0, 0 ), new int2( 1, 1 ) };
			ExpectedPath( way, expectedPath );
		}

		[Test]
		public void OneCost_Right()
		{
			CreateMap( new float[] {
				MovementCost.FREE, MovementCost.FREE,
				MovementCost.FREE, MovementCost.FREE,
			} );

			var request = _entityManager.CreateEntity(typeof(PathRequest));
			_entityManager.SetComponentData( request, new PathRequest { Start = new int2( 0, 0 ), End = new int2( 1, 1 ) } );

			Update();

			var way = _entityManager.GetBuffer<Waypoint>( request );
			PrintWay( way );
			Assert.True( _entityManager.GetComponentData<PathRequest>( request ).Done );
			Assert.AreEqual( _entityManager.GetBuffer<Waypoint>( request ).Length, 2 );

			var expectedPath = new int2[] { new int2( 0, 0 ), new int2( 1, 1 ) };
			ExpectedPath( way, expectedPath );
		}

		[Test]
		public void OneCost_Diagonal()
		{
			CreateMap( new float[] {
				MovementCost.FREE, MovementCost.FREE, MovementCost.FREE,
				MovementCost.FREE, MovementCost.FREE, MovementCost.FREE,
				MovementCost.FREE, MovementCost.FREE, MovementCost.FREE,
			} );

			var request = _entityManager.CreateEntity(typeof(PathRequest));
			_entityManager.SetComponentData( request, new PathRequest { Start = new int2( 0, 0 ), End = new int2( 2, 2 ) } );

			Update();

			var way = _entityManager.GetBuffer<Waypoint>( request );
			PrintWay( way );
			Assert.True( _entityManager.GetComponentData<PathRequest>( request ).Done );

			var expectedPath = new int2[] { new int2( 0, 0 ), new int2( 1, 1 ), new int2(2, 2) };
			ExpectedPath( way, expectedPath );
		}

		[Test]
		public void Avoid()
		{
			CreateMap( new float[] {
				1, 1, 1,
				1, 5, 1,
				1, 1, 1,
			} );

			var request = _entityManager.CreateEntity(typeof(PathRequest));
			_entityManager.SetComponentData( request, new PathRequest { Start = new int2( 0, 0 ), End = new int2( 2, 2 ) } );

			Update();

			var way = _entityManager.GetBuffer<Waypoint>( request );
			PrintWay( way );
			Assert.True( _entityManager.GetComponentData<PathRequest>( request ).Done );

			var expectedPath1 = new int2[] { new int2( 0, 0 ), new int2( 1, 0 ), new int2(2, 1), new int2(2, 2) };
			var expectedPath2 = new int2[] { new int2( 0, 0 ), new int2( 0, 1 ), new int2(1, 2), new int2(2, 2) };

			ExpectedPath( way, expectedPath1, expectedPath2 );
		}

		#region Helpers

		private void ExpectedPath( DynamicBuffer<Waypoint> way, params int2[][] expectedPath )
		{
			for ( int j = 0; j < expectedPath.Length; j++ )
			{
				Assert.AreEqual( expectedPath[j].Length, way.Length );

				// Reverse path because way is in reverse manner
				Array.Reverse( expectedPath[j] );

				int counter = 0;
				for ( int i = 0; i < way.Length; i++ )
				{
					// To big distance, there will be no match
					if ( math.distance( way[i].Position, expectedPath[j][i] ) > 0.1f )
					{
						break;
					}
					// So far good, so update counter
					++counter;
				}

				// We found matching pair
				if ( counter == way.Length )
				{
					return;
				}
			}
		}

		private void PrintWay( DynamicBuffer<Waypoint> dynamicBuffer )
		{
			StringBuilder sb = new StringBuilder("Map:");
			for ( int i = 0; i < dynamicBuffer.Length; i++ )
			{
				sb.Append( dynamicBuffer[i].Position );
				sb.Append( ", " );
			}
			Debug.Log( sb.ToString() );
		}

		private void CreateMap( float[] map )
		{
			var mapSize = map.Length;

			var mapArchetype = _entityManager.CreateArchetype(typeof(MapSettings));
			var tileArchetype = _entityManager.CreateArchetype(typeof(MovementCost));
			var tiles = new NativeArray<Entity>(mapSize, Allocator.Temp);

			_entityManager.CreateEntity( tileArchetype, tiles );
			var mapEntity = _entityManager.CreateEntity(mapArchetype);

			for ( int i = 0; i < mapSize; i++ )
			{
				_entityManager.SetComponentData( tiles[i], new MovementCost { Cost = map[i] } );
			}

			var blitableTiles = new BlitableArray<Entity>();
			blitableTiles.Allocate( tiles, Allocator.Temp );

			TargetSystem.SetSingleton( new MapSettings { MapEdgeSize = EdgeSize( mapSize ), Tiles = blitableTiles } );
		}

		#endregion Helpers
	}
}