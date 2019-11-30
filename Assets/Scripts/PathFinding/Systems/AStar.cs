using Helpers;

using Maps.Components;

using PathFinding.Components;
using PathFinding.Helpers;

using System.Runtime.CompilerServices;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;

using static Helpers.IndexUtils;

namespace PathFinding.Systems
{
	public class AStar : ComponentSystem
	{
		#region ProfilerMarkers
		private static ProfilerMarker _markerAStar        = new ProfilerMarker("AStar.System");
		private static ProfilerMarker _markerSetup        = new ProfilerMarker("AStar.Setup");
		private static ProfilerMarker _markerSetupData    = new ProfilerMarker("AStar.Setup_data");
		private static ProfilerMarker _markerSearch       = new ProfilerMarker("AStar.Search");
		private static ProfilerMarker _markerReconstruct  = new ProfilerMarker("AStar.Reconstruct");
		private static ProfilerMarker _markerCleanup      = new ProfilerMarker("AStar.Cleanup");
		private static ProfilerMarker _markerFindCurrent  = new ProfilerMarker("AStar.Find_current");
		private static ProfilerMarker _markerMovementData = new ProfilerMarker("AStar.Movement_data");
		#endregion ProfilerMarkers

		private EndSimulationEntityCommandBufferSystem _eseCommandBufferSystem;

		#region Lifetime

		protected override void OnCreate() => _eseCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

		protected override void OnDestroy() => Entities.ForEach( ( ref MapSettingsNeighborsState neighboursState ) =>
											{
												neighboursState.Neighbours.Dispose();
											} );

		protected override void OnUpdate()
		{
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

			Entities.WithNone<Waypoint>().ForEach( ( Entity requestEntity, ref PathRequest pathRequest ) =>
			{
				// Request already completed
				// TODO: Split request from waypoints
				if ( pathRequest.Done )
				{
					return;
				}

				_markerAStar.Begin();

				#region Setup

				_markerSetup.Begin();
				NativeArray<float2> costs = new NativeArray<float2>( tilesSize, Allocator.Temp );
				NativeArray<Boolean> closeSet = new NativeArray<Boolean>( tilesSize, Allocator.Temp );
				NativeArray<int> camesFrom = new NativeArray<int>( tilesSize, Allocator.Temp );
				NativeMinHeap minSet = new NativeMinHeap(tilesSize, Allocator.Temp);

				_markerSetupData.Begin();
				// Setup initial data
				for ( int i = 0; i < tilesSize; i++ )
				{
					costs[i] = new float2 { x = 0, y = float.MaxValue };
					camesFrom[i] = -1;
				}
				_markerSetupData.End();

				// Shortcuts
				int startTile = Index1D( pathRequest.Start, tilesSize );
				int endTile = Index1D( pathRequest.End, tilesSize );

				costs[startTile] = 0;
				_markerSetup.End();

				#endregion Setup

				#region Search

				_markerSearch.Begin();
				int lastTile = startTile;
				// While not in destination and not at dead end
				while ( lastTile != endTile && lastTile != -1 )
				{
					// Mark current tile as visited
					closeSet[lastTile] = true;

					_markerMovementData.Begin();
					var currentCost = movementData[tiles[lastTile]];
					_markerMovementData.End();

					for ( int i = 0; i < neighbors.Length; ++i )
					{
						var neighbor = neighbors[i];
						// Find linear neighbor index
						var neighborIndex = neighbor.Of( lastTile, tilesSize );
						// Check if neighbor exists
						if ( neighborIndex != -1 )
						{
							// Previous + current cost
							var costG = costs[lastTile].x + neighbor.Distance * currentCost.Cost;
							var costF = costG + Heuristic( pathRequest, neighborIndex, tilesSize );

							if ( costs[neighborIndex].y > costF )
							{
								// Update cost and path
								costs[neighborIndex] = new float2( costG, costF );
								camesFrom[neighborIndex] = lastTile;

								// Update min set
								if ( closeSet[neighborIndex] == false )
								{
									minSet.Push( new MinHeapNode( neighborIndex, costF ) );
								}
							}
						}
					}

					lastTile = FindCurrent( minSet, closeSet );
				}
				_markerSearch.End();

				#endregion Search

				#region ReconstructPath

				_markerReconstruct.Begin();
				var waypoints = commandBuffer.AddBuffer<Waypoint>( requestEntity );
				// Travel back through path
				while ( lastTile != -1 )
				{
					waypoints.Add( new Waypoint() { Position = Index2D( lastTile, tilesSize ) } );
					lastTile = camesFrom[lastTile];
				}
				_markerReconstruct.End();

				#endregion ReconstructPath

				#region Cleanup

				_markerCleanup.Begin();
				// Dispose all temporary data
				costs.Dispose();
				closeSet.Dispose();
				camesFrom.Dispose();
				minSet.Dispose();
				// Mark request as completed
				pathRequest.Done = true;
				_markerCleanup.End();

				#endregion Cleanup

				_markerAStar.End();
			} );
		}

		#endregion Lifetime

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static int FindCurrent( NativeMinHeap minSet, NativeArray<Boolean> closeSet )
		{
			_markerFindCurrent.Begin();
			while ( minSet.HasNext() )
			{
				var next = minSet.Pop();
				// Check if this is not visited tile
				if ( closeSet[next.Position] == false )
				{
					_markerFindCurrent.End();
					return next.Position;
				}
			}
			_markerFindCurrent.End();
			return -1;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private float Heuristic( PathRequest pathRequest, int neighborIndex, int TilesSize )
		{
			var index2D = Index2D( neighborIndex, TilesSize );
			return math.abs( pathRequest.End.x - index2D.x ) + math.abs( pathRequest.End.y - index2D.y );
		}
	}
}