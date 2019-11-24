using Helpers;

using Maps.Components;

using PathFinding.Components;
using PathFinding.Helpers;

using System.Runtime.CompilerServices;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

using UnityEngine.Profiling;

using static Helpers.IndexUtils;

namespace PathFinding.Systems
{
	public class AStar : ComponentSystem
	{
		private EndSimulationEntityCommandBufferSystem _eseCommandBufferSystem;

		#region Lifetime

		protected override void OnCreate() => _eseCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

		protected override void OnDestroy() => Entities.ForEach( ( ref MapSettingsNeighborsState neighboursState ) =>
											{
												neighboursState.Neighbours.Dispose();
											} );

		protected override void OnUpdate()
		{
			CheckNeighborsState();

			BlitableArray<Entity> tiles = new BlitableArray<Entity>();
			BlitableArray<Neighbor> neighbors = new BlitableArray<Neighbor>();

			Entities.ForEach( ( Entity e, ref MapSettings mapSettings, ref MapSettingsNeighborsState neighboursState ) =>
			{
				neighbors = neighboursState.Neighbours;
				tiles = mapSettings.Tiles;
			} );
			if ( tiles.Length < 1 || neighbors.Length < 1 )
			{
				return;
			}

			var movementComponents = GetComponentDataFromEntity<MovementCost>( true );
			var tilesSize = tiles.Length;
			var commandBuffer = _eseCommandBufferSystem.CreateCommandBuffer();
			var movementData = movementComponents;

			Entities.WithNone<Waypoint>().ForEach( ( Entity e, ref PathRequest pathRequest ) =>
			{
				if ( pathRequest.Done )
				{
					return;
				}

				Profiler.BeginSample( "AStar" );

				#region Setup

				Profiler.BeginSample( "AStar_Setup" );
				NativeArray<float2> costs = new NativeArray<float2>( tilesSize, Allocator.Temp );
				NativeArray<Boolean> closeSet = new NativeArray<Boolean>( tilesSize, Allocator.Temp );
				NativeArray<int> camesFrom = new NativeArray<int>( tilesSize, Allocator.Temp );

				for ( int i = 0; i < tilesSize; i++ )
				{
					costs[i] = new float2 { x = 0, y = float.MaxValue };
					camesFrom[i] = -1;
				}

				int startTile = Index1D( pathRequest.Start, tilesSize );
				int endTile = Index1D( pathRequest.End, tilesSize );

				costs[startTile] = 0;
				Profiler.EndSample();

				#endregion Setup

				#region Search

				Profiler.BeginSample( "AStar_Search" );
				int lastTile;
				while ( true )
				{
					var current = FindCurrent(costs, closeSet, tilesSize);
					//There is no way, all tiles visited
					if ( current == tilesSize || current == endTile )
					{
						lastTile = current;
						break;
					}

					closeSet[current] = true;
					var currentCost = movementData[tiles[current]];

					for ( int i = 0; i < neighbors.Length; ++i )
					{
						var neighbor = neighbors[i];
						var neighborIndex = neighbor.Of( current, tilesSize );
						if ( neighborIndex == -1 )
						{
							continue;
						}

						// Previous + current cost
						var costG = costs[current].x + neighbor.Distance * currentCost.Cost;
						var costF = costG + Heuristic( pathRequest, neighborIndex, tilesSize );

						if ( costs[neighborIndex].y > costF )
						{
							costs[neighborIndex] = new float2( costG, costF );
							camesFrom[neighborIndex] = current;
						}
					}
				}
				Profiler.EndSample();

				#endregion Search

				#region ReconstructPath

				Profiler.BeginSample( "AStar_ReconstructPath" );
				var waypoints = commandBuffer.AddBuffer<Waypoint>( e );
				while ( lastTile != -1 )
				{
					waypoints.Add( new Waypoint() { Position = Index2D( lastTile, tilesSize ) } );
					lastTile = camesFrom[lastTile];
				}
				Profiler.EndSample();

				#endregion ReconstructPath

				#region Cleanup

				Profiler.BeginSample( "AStar_Cleanup" );
				costs.Dispose();
				closeSet.Dispose();
				camesFrom.Dispose();
				pathRequest.Done = true;
				Profiler.EndSample();

				#endregion Cleanup

				Profiler.EndSample();
			} );
		}

		#endregion Lifetime

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static int FindCurrent( NativeArray<float2> costs, NativeArray<Boolean> closeSet, int tileSize )
		{
			Profiler.BeginSample( "AStar_FindCurrent" );
			int current = 0;

			while ( current < tileSize && closeSet[current] )
			{
				current++;
			}

			for ( int i = current; i < tileSize; i++ )
			{
				if ( closeSet[i] == false && costs[i].y < costs[current].y )
				{
					current = i;
				}
			}
			Profiler.EndSample();
			return current;
		}

		private void CheckNeighborsState() => Entities.WithNone<MapSettingsNeighborsState>().ForEach( ( Entity e, ref MapSettings mapSettings ) =>
						{
							MapSettingsNeighborsState neighboursState = new MapSettingsNeighborsState();
							if ( mapSettings.CanMoveDiagonally )
							{
								neighboursState.Neighbours = new BlitableArray<Neighbor>( 8, Allocator.Persistent )
								{
									[0] = Neighbor.UPPER_LEFT,
									[1] = Neighbor.UPPER,
									[2] = Neighbor.UPPER_RIGHT,
									[3] = Neighbor.LEFT,
									[4] = Neighbor.RIGHT,
									[5] = Neighbor.BOTTOM_LEFT,
									[6] = Neighbor.BOTTOM,
									[7] = Neighbor.BOTTOM_RIGHT,
								};
							}
							else
							{
								neighboursState.Neighbours = new BlitableArray<Neighbor>( 4, Allocator.Persistent )
								{
									[0] = Neighbor.UPPER,
									[1] = Neighbor.LEFT,
									[2] = Neighbor.RIGHT,
									[3] = Neighbor.BOTTOM,
								};
							}
							PostUpdateCommands.AddComponent( e, neighboursState );
						} );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private float Heuristic( PathRequest pathRequest, int neighborIndex, int TilesSize )
		{
			var index2D = Index2D( neighborIndex, TilesSize );
			return math.abs( pathRequest.End.x - index2D.x ) + math.abs( pathRequest.End.y - index2D.y );
		}
	}
}