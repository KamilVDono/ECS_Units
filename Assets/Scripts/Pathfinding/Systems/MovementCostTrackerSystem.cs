using Helpers.Types;

using Maps.Components;
using Maps.Systems;

using Pathfinding.Components;

using Resources.Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

using UnityEngine;

namespace Pathfinding.Systems
{
	/// <summary>
	/// Track move-through tile cost. New cost will be applied just before end of simulation <see cref="EndSimulationEntityCommandBufferSystem"/>
	/// </summary>
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
			Debug.Log( $"Create" );
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
			if ( _changedEntities.IsCreated )
			{
				_changedEntities.Dispose();
			}
		}

		protected override JobHandle OnUpdate( JobHandle inputDependencies )
		{
			// calculate minimum allocation size
			int maxEntities = _groundChangeQuery.CalculateEntityCount() + _resourceChangeQuery.CalculateEntityCount() + _stockChangeQuery.CalculateEntityCount();

			if ( maxEntities < 1 )
			{
				return inputDependencies;
			}

			// check if need create map
			if ( _changedEntities.IsCreated == false )
			{
				// one big allocation so we are sure we have capacity to store whole map
				_changedEntities = new NativeHashMap<Entity, Boolean>( GetSingleton<MapSettings>().Tiles.Length, Allocator.Persistent );
			}

			// setup to collect changed entities
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
					// just go through entities
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

			// local shallow copy for job (jobs requirement)
			var changedEntities = _changedEntities;

			// setup data
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
					// obtain entities
					var entities = changedEntities.GetKeyArray(Allocator.Temp);
					for(int i = 0; i < entities.Length; i++ )
					{
						// calculate full movement cost
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

						// apply new cost
						applyCB.SetComponent(i, entity, new MovementCost()
						{
							Cost = resourceOreCost + groundCost + stockCost
						} );
					}
					entities.Dispose();
				}).Schedule(changeFillHandle);

			_removeCmdBufferSystem.AddJobHandleForProducer( applyJobHandle );
			return applyJobHandle;
		}
	}
}