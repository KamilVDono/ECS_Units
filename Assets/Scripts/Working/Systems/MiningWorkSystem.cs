using Maps.Components;
using Maps.Systems;

using Pathfinding.Helpers;

using Resources.Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

using Working.Components;

namespace Working.Systems
{
	[UpdateAfter( typeof( WorkProgressionSystem ) )]
	public class MiningWorkSystem : JobComponentSystem, IRequiresMapSettings
	{
		private EndSimulationEntityCommandBufferSystem _removeCmdBufferSystem;
		private NativeArray<Neighbor> _neighbors;

		public Entity MapSettingsEntity { get; set; }

		protected override void OnCreate()
		{
			_removeCmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
			_neighbors = Neighbor.FullNeighborhood( Allocator.Persistent );
		}

		protected override JobHandle OnUpdate( JobHandle inputDeps )
		{
			MapSettings mapSettings = new MapSettings();
			EntityManager entityManager = EntityManager;
			if ( EntityManager.Exists( MapSettingsEntity ) )
			{
				mapSettings = EntityManager.GetSharedComponentData<MapSettings>( MapSettingsEntity );
			}

			if ( mapSettings.Tiles.Length < 1 )
			{
				return inputDeps;
			}

			var neighbor = _neighbors;
			var cmdBuffer = _removeCmdBufferSystem.CreateCommandBuffer().ToConcurrent();

			var stocks = GetComponentDataFromEntity<Stock>(true);
			var ores = GetComponentDataFromEntity<ResourceOre>(true);

			// TODO: Allow burst
			var jobHandle = Entities.WithAll<MiningWork>()
				.WithReadOnly(neighbor)
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

						bool spawnedNewStock = false;
						int i = 0;
						while ( spawnedNewStock && i < neighbor.Length )
						{
							var neighborIndex = neighbor[i].Of(index.Index1D, mapSettings.MapEdgeSize);
							if ( neighborIndex != -1 )
							{
								var neighborEntity = mapSettings.Tiles[neighborIndex];

								if ( ores.HasComponent( neighborEntity ) == false || ores[neighborEntity].IsValid == false )
								{
									if ( stocks.HasComponent( neighborEntity) )
									{
										cmdBuffer.AddComponent( entityInQueryIndex, mapSettings.Tiles[neighborIndex], new Stock() { Count = 1, MaxSize = 500, Type = ore.Type } );
										spawnedNewStock = true;
									}
								}
							}
							i++;
						}
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

		protected override void OnDestroy() => _neighbors.Dispose();
	}
}