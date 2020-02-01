using Helpers;

using Maps.Components;
using Maps.Systems;

using Pathfinding.Components;

using Resources.Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Pathfinding.Systems
{
	[UpdateAfter( typeof( MapSpawner ) )]
	[UpdateInGroup( typeof( InitializationSystemGroup ) )]
	public class MovementCostTrackerSystem : JobComponentSystem
	{
		private EntityQuery _groundChangeQuery;
		private EntityQuery _resourceChangeQuery;
		private EntityQuery _stockChangeQuery;
		private EndSimulationEntityCommandBufferSystem _removeCmdBufferSystem;
		private NativeHashMap< Entity, Boolean > _changedEntities;

		protected override void OnCreate()
		{
			_groundChangeQuery = GetEntityQuery( ComponentType.ReadOnly<MovementCost>(), ComponentType.ReadOnly<GroundType>(), ComponentType.ReadOnly<ResourceOre>() );
			_groundChangeQuery.SetChangedVersionFilter( typeof( GroundType ) );

			_resourceChangeQuery = GetEntityQuery( ComponentType.ReadOnly<MovementCost>(), ComponentType.ReadOnly<GroundType>(), ComponentType.ReadOnly<ResourceOre>() );
			_resourceChangeQuery.SetChangedVersionFilter( typeof( ResourceOre ) );

			_stockChangeQuery = GetEntityQuery( ComponentType.ReadOnly<MovementCost>(), ComponentType.ReadOnly<Stock>() );
			_stockChangeQuery.SetChangedVersionFilter( typeof( Stock ) );

			_removeCmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
			RequireSingletonForUpdate<MapSettings>();
		}

		protected override void OnDestroy()
		{
			_changedEntities.Dispose();
		}

		protected override JobHandle OnUpdate( JobHandle inputDependencies )
		{
			int maxEntities = _groundChangeQuery.CalculateEntityCount() + _resourceChangeQuery.CalculateEntityCount() + _stockChangeQuery.CalculateEntityCount();

			if ( maxEntities < 1 )
			{
				return inputDependencies;
			}

			if ( _changedEntities.IsCreated == false )
			{
				_changedEntities = new NativeHashMap<Entity, Boolean>( GetSingleton<MapSettings>().Tiles.Length, Allocator.Persistent );
			}

			_changedEntities.Clear();
			var groundEntities = _groundChangeQuery.ToEntityArray(Allocator.TempJob);
			var resourceEntities = _resourceChangeQuery.ToEntityArray(Allocator.TempJob);
			var stockEntities = _stockChangeQuery.ToEntityArray(Allocator.TempJob);

			var changedEntitiesWriter = _changedEntities.AsParallelWriter();

			var changeFillHandle = Job
				.WithReadOnly(groundEntities)
				.WithReadOnly(resourceEntities)
				.WithReadOnly(stockEntities)
				.WithDeallocateOnJobCompletion( groundEntities )
				.WithDeallocateOnJobCompletion( resourceEntities )
				.WithDeallocateOnJobCompletion( stockEntities )
				.WithCode( () =>
				{
					for(int i = 0; i < groundEntities.Length; i++ )
					{
						changedEntitiesWriter.TryAdd( groundEntities[i], true );
					}
					for(int i = 0; i < resourceEntities.Length; i++ )
					{
						changedEntitiesWriter.TryAdd( resourceEntities[i], true );
					}
					for(int i = 0; i < stockEntities.Length; i++ )
					{
						changedEntitiesWriter.TryAdd( stockEntities[i], true );
					}
				} ).Schedule( inputDependencies );

			var changedEntities = _changedEntities;

			var grounds = GetComponentDataFromEntity<GroundType>(true);
			var resourceOres = GetComponentDataFromEntity<ResourceOre>(true);
			var stocks = GetComponentDataFromEntity<Stock>(true);
			var applyCB = _removeCmdBufferSystem.CreateCommandBuffer().ToConcurrent();

			var applyJobHandle = Job
				.WithReadOnly(grounds)
				.WithReadOnly(resourceOres)
				.WithReadOnly(stocks)
				.WithCode(
				() =>
				{
					var entities = changedEntities.GetKeyArray(Allocator.Temp);
					for(int i = 0; i < entities.Length; i++ )
					{
						var entity = entities[i];
						var resourceOre = resourceOres[entity];
						var resourceOreCost = 0f;
						if (resourceOre.IsValid)
						{
							resourceOreCost = resourceOre.Type.Value.MovementCost;
						}

						var groundCost = grounds[entity].TileTypeBlob.Value.MoveCost;

						var stockCost = 0f;
						if (stocks.HasComponent( entity ) )
						{
							stockCost = stocks[entity].Type.Value.MovementCost;
						}

						applyCB.SetComponent(i, entity, new MovementCost()
						{
							Cost = resourceOreCost + groundCost + stockCost
						} );
					}
				}).Schedule(changeFillHandle);

			_removeCmdBufferSystem.AddJobHandleForProducer( applyJobHandle );
			return applyJobHandle;
		}
	}
}