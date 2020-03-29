using Helpers;
using Helpers.Types;

using Maps.Components;

using Pathfinding.Components;

using Resources.Components;

using StateMachine.Components;

using Units.Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Units.Systems
{
	public class UnitTagetSettingSystem : SystemBase
	{
		private EndSimulationEntityCommandBufferSystem _cmdBufferSystem;
		private NativeArray<Neighbor> _neighbors;

		protected override void OnCreate()
		{
			RequireSingletonForUpdate<MapSettings>();
			_cmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
			_neighbors = Neighbor.FullNeighborhood( Allocator.Persistent );
		}

		protected override void OnUpdate()
		{
			var mapSettings = GetSingleton<MapSettings>();
			var edgeSize = mapSettings.MapEdgeSize;

			var ores = GetComponentDataFromEntity<ResourceOre>( true );

			var tiles = mapSettings.Tiles;
			var neighborsLocal = _neighbors;

			var setTargetCMDBuffer = _cmdBufferSystem.CreateCommandBuffer().ToConcurrent();

			// TODO: Enable burst (native queue allocation problem)
			Entities
				.WithReadOnly( ores )
				.WithReadOnly( neighborsLocal )
				.WithReadOnly( tiles )
				.WithNativeDisableContainerSafetyRestriction( tiles )
				.WithoutBurst()
				.WithAll<UnitTag, SeekingOresTag>()
				.WithNone<Waypoint, PathRequest>()
				.ForEach( ( Entity e, int entityInQueryIndex, in MapIndex mapIndex ) =>
				{
					#region Found ores
					int2 oreIndex = new int2();

					bool founded = false;
					var tilesSize = tiles.Length;

					NativeQueue<int> toSearch = new NativeQueue<int>(Allocator.Temp);
					NativeArray<Boolean> searched = new NativeArray<Boolean>(tiles.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
					toSearch.Enqueue( mapIndex.Index1D );

					while ( founded == false && toSearch.Count > 0 )
					{
						var currentIndex = toSearch.Dequeue();
						for ( int i = 0; i < neighborsLocal.Length; i++ )
						{
							var neighborIndex = neighborsLocal[i].Of(currentIndex, tilesSize);
							if ( neighborIndex != -1 && searched[neighborIndex] == false )
							{
								toSearch.Enqueue( neighborIndex );
							}
						}

						var currentEntity = tiles[currentIndex];
						searched[currentIndex] = true;

						if ( ores.HasComponent( currentEntity ) && ores[currentEntity].IsValid )
						{
							oreIndex = IndexUtils.Index2D( currentIndex, tilesSize );
							founded = true;
						}
					}

					toSearch.Dispose();
					searched.Dispose();
					#endregion Found ores

					setTargetCMDBuffer.AddComponent<PathRequest>( entityInQueryIndex, e, new PathRequest( mapIndex.Index2D, oreIndex ) );
				} ).ScheduleParallel();

			_cmdBufferSystem.AddJobHandleForProducer( Dependency );
		}

		protected override void OnDestroy() => _neighbors.Dispose();
	}
}