﻿using Helpers;

using Maps.Components;

using Pathfinding.Components;
using Pathfinding.Helpers;

using System.Runtime.CompilerServices;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;

using static Helpers.IndexUtils;

namespace Pathfinding.Systems
{
	/// <summary>
	/// A* system Uses:
	/// <list type="bullet">
	/// <item><see cref="MapSettings"/> for map tiles</item>
	/// <item><see cref="MovementCost"/> for cost calculation</item>
	/// <item><see cref="PathRequest"/> for request data</item>
	/// <item><see cref="Waypoint"/> for store response path</item>
	/// </list>
	/// </summary>
	public class AStar : ComponentSystem
	{
		#region ProfilerMarkers
		private static ProfilerMarker _markerAStar        = new ProfilerMarker("AStar.System");
		private static ProfilerMarker _markerSetup        = new ProfilerMarker("AStar.Setup");
		private static ProfilerMarker _markerSetupData    = new ProfilerMarker("AStar.Setup_data");
		private static ProfilerMarker _markerSetupDataMovement    = new ProfilerMarker("AStar.Setup_data_movement");
		private static ProfilerMarker _markerSearch       = new ProfilerMarker("AStar.Search");
		private static ProfilerMarker _markerReconstruct  = new ProfilerMarker("AStar.Reconstruct");
		private static ProfilerMarker _markerCleanup      = new ProfilerMarker("AStar.Cleanup");
		private static ProfilerMarker _markerFindCurrent  = new ProfilerMarker("AStar.Find_current");
		private static ProfilerMarker _markerMovementData = new ProfilerMarker("AStar.Movement_data");
		private static ProfilerMarker _markerPop = new ProfilerMarker("AStar.Pop");
		#endregion ProfilerMarkers

		private EndSimulationEntityCommandBufferSystem _eseCommandBufferSystem;
		private NativeArray<Neighbor> _neighbors;

		#region Lifetime

		// Obtain buffer
		protected override void OnCreate()
		{
			_eseCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
			_neighbors = Neighbor.FullNeighborhood( Allocator.Persistent );
		}

		protected override void OnUpdate()
		{
			var mapSettings = GetSingleton<MapSettings>();

			// The data is in not valid state
			if ( mapSettings.Tiles.Length < 1 )
			{
				return;
			}

			// Other data
			var movementComponents = GetComponentDataFromEntity<MovementCost>( true );
			var tilesSize = mapSettings.Tiles.Length;
			var commandBuffer = _eseCommandBufferSystem.CreateCommandBuffer();


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
				NativeArray<float2> costs = new NativeArray<float2>( tilesSize, Allocator.Temp, NativeArrayOptions.UninitializedMemory );
				NativeArray<Boolean> closeSet = new NativeArray<Boolean>( tilesSize, Allocator.Temp, NativeArrayOptions.UninitializedMemory );
				NativeArray<int> camesFrom = new NativeArray<int>( tilesSize, Allocator.Temp, NativeArrayOptions.UninitializedMemory );
				NativeMinHeap minSet = new NativeMinHeap(tilesSize, Allocator.Temp);

				_markerSetupData.Begin();
				// Setup initial data
				for ( int i = 0; i < tilesSize; i++ )
				{
					costs[i] = new float2 { x = 0, y = float.MaxValue };
					camesFrom[i] = -1;
					closeSet[i] = false;
				}

				var movementData = new NativeArray<MovementCost>( mapSettings.Tiles.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory );

				_markerSetupDataMovement.Begin();
				for ( int i = 0; i < tilesSize; i++ )
				{
					movementData[i] = EntityManager.GetComponentData<MovementCost>( mapSettings.Tiles[i] );
				}
				_markerSetupDataMovement.End();

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

					for ( int i = 0; i < _neighbors.Length; ++i )
					{
						var neighbor = _neighbors[i];
						// Find linear neighbor index
						var neighborIndex = neighbor.Of( lastTile, tilesSize );
						// Check if neighbor exists
						if ( neighborIndex != -1 )
						{
							// Previous + current cost
							_markerMovementData.Begin();
							var currentCost = movementData[neighborIndex];
							_markerMovementData.End();

							if ( currentCost.Cost == MovementCost.IMPOSSIBLE )
							{
								closeSet[neighborIndex] = true;
								continue;
							}

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
				movementData.Dispose();
				// Mark request as completed
				pathRequest.Done = true;
				_markerCleanup.End();

				#endregion Cleanup

				_markerAStar.End();
			} );


		}

		protected override void OnDestroy()
		{
			_neighbors.Dispose();
		}

		#endregion Lifetime

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static int FindCurrent( NativeMinHeap minSet, NativeArray<Boolean> closeSet )
		{
			_markerFindCurrent.Begin();
			while ( minSet.HasNext() )
			{
				_markerPop.Begin();
				var next = minSet.Pop();
				_markerPop.End();
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