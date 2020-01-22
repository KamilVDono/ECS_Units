using Helpers;

using Maps.Components;

using Pathfinding.Helpers;

using Resources.Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

using Working.Components;

namespace Working.Systems
{
	/// <summary>
	/// </summary>
	[UpdateAfter( typeof( WorkProgressionSystem ) )]
	public class MiningWorkSystem : JobComponentSystem
	{
		private EndSimulationEntityCommandBufferSystem _removeCmdBufferSystem;
		private NativeArray<Neighbor> _neighbors;

		protected override void OnCreate()
		{
			_removeCmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
			_neighbors = Neighbor.FullNeighborhood( Allocator.Persistent );
		}

		protected override JobHandle OnUpdate( JobHandle inputDeps )
		{
			MapSettings mapSettings = GetSingleton<MapSettings>();

			if ( mapSettings.Tiles.Length < 1 )
			{
				return inputDeps;
			}

			var neighbors = _neighbors;
			var cmdBuffer = _removeCmdBufferSystem.CreateCommandBuffer().ToConcurrent();

			var stocks = GetComponentDataFromEntity<Stock>(true);
			var ores = GetComponentDataFromEntity<ResourceOre>(true);

			var jobHandle = Entities.WithAll<MiningWork>()
				.WithReadOnly(neighbors)
				.ForEach( ( Entity e, int entityInQueryIndex, ref WorkProgress workProgress, ref ResourceOre ore, in MapIndex index ) =>
			{
				if ( ore.IsValid )
				{
					int oresMined = 0;
					while ( workProgress.Progress > ore.Type.Value.WorkRequired )
					{
						workProgress.Progress -= ore.Type.Value.WorkRequired;
						oresMined++;
						ore.Count--;

						SpawnMinedOre( entityInQueryIndex, ore, in index, in mapSettings, in neighbors, in cmdBuffer, in stocks, in ores );
					}

					if ( ore.Count < 0 )
					{
						ore.Count = 0;
						cmdBuffer.RemoveComponent<MiningWork>( entityInQueryIndex, e );
					}
				}
			} ).Schedule( inputDeps );

			_removeCmdBufferSystem.AddJobHandleForProducer( jobHandle );
			return jobHandle;
		}

		protected override void OnDestroy()
		{
			_neighbors.Dispose();
		}

		private static void SpawnMinedOre( int entityInQueryIndex, ResourceOre ore, in MapIndex index, in MapSettings mapSettings, in NativeArray<Neighbor> neighbors, in EntityCommandBuffer.Concurrent cmdBuffer, in ComponentDataFromEntity<Stock> stocks, in ComponentDataFromEntity<ResourceOre> ores )
		{
			if ( FindNearestStock( in index, in mapSettings, in neighbors, in stocks, in ores, out Entity stockTile, out bool asNew ) )
			{
				if ( asNew )
				{
					cmdBuffer.AddComponent( entityInQueryIndex, stockTile, new Stock() { Count = 1, MaxSize = 500, Type = ore.Type } );
				}
				else
				{
					cmdBuffer.AddComponent( entityInQueryIndex, stockTile, new StockCountChange() { Count = 1 } );
				}
			}
		}

		private static bool FindNearestStock( in MapIndex index, in MapSettings mapSettings, in NativeArray<Neighbor> neighbors, in ComponentDataFromEntity<Stock> stocks, in ComponentDataFromEntity<ResourceOre> ores, out Entity stock, out bool asNew )
		{
			stock = new Entity();
			asNew = true;

			bool founded = false;

			NativeQueue<int> toSearch = new NativeQueue<int>(Allocator.Temp);
			NativeArray<Boolean> searched = new NativeArray<Boolean>(neighbors.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
			toSearch.Enqueue( index.Index1D );

			while ( founded == false || toSearch.Count < 1 )
			{
				var currentIndex = toSearch.Dequeue();
				for ( int i = 0; i < neighbors.Length; i++ )
				{
					var neighborIndex = neighbors[i].Of(index.Index1D, mapSettings.MapEdgeSize);
					if ( neighborIndex != -1 && searched[neighborIndex] == false )
					{
						toSearch.Enqueue( neighborIndex );
					}
				}

				var currentEntity = mapSettings.Tiles[currentIndex];

				if ( ores.HasComponent( currentEntity ) == false || ores[currentEntity].IsValid == false )
				{
					if ( stocks.HasComponent( currentEntity ) == false )
					{
						stock = currentEntity;
						asNew = true;
						founded = true;
					}
				}
			}

			toSearch.Dispose();
			searched.Dispose();

			return founded;
		}
	}
}