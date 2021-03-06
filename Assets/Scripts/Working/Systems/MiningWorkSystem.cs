﻿using Helpers.Types;

using Maps.Components;

using Resources.Components;

using StateMachine.Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

using Working.Components;

namespace Working.Systems
{
	[UpdateAfter( typeof( WorkProgressionSystem ) )]
	public class MiningWorkSystem : SystemBase
	{
		private EndSimulationEntityCommandBufferSystem _removeCmdBufferSystem;
		private NativeArray<Neighbor> _neighbors;
		private EntityArchetype _minedArchetype;
		private EntityArchetype _changedOreArchetype;

		protected override void OnCreate()
		{
			RequireSingletonForUpdate<MapSettings>();
			_removeCmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
			_neighbors = Neighbor.FullNeighborhood( Allocator.Persistent );
			_minedArchetype = EntityManager.CreateArchetype( typeof( MinedOre ), typeof( MapIndex ) );
			_changedOreArchetype = EntityManager.CreateArchetype( typeof( ResourceOreChange ) );
		}

		// TODO: More check

		protected override void OnUpdate()
		{
			MapSettings mapSettings = GetSingleton<MapSettings>();

			if ( mapSettings.Tiles.Length < 1 )
			{
				return;
			}

			var miningProgressCB = _removeCmdBufferSystem.CreateCommandBuffer().ToConcurrent();
			var spawnStocksCB = _removeCmdBufferSystem.CreateCommandBuffer().ToConcurrent();

			var minedArchetypeLocal = _minedArchetype;
			var changedOreArchetypeLocal = _changedOreArchetype;
			var miningTags = GetComponentDataFromEntity<MiningTag>(true);

			Entities
				.WithChangeFilter<WorkProgress>()
				.WithoutBurst()
				.WithReadOnly( miningTags )
				.ForEach( ( Entity e, int entityInQueryIndex, ref WorkProgress workProgress, ref ResourceOre ore, in MapIndex index, in MiningWork miningWork ) =>
				{
					if ( ore.IsValid && ore.Type.Value.WorkRequired > 0 )
					{
						int oresMined = 0;
						while ( workProgress.Progress >= ore.Type.Value.WorkRequired )
						{
							workProgress.Progress -= ore.Type.Value.WorkRequired;
							oresMined++;
							ore.Count--;
						}

						if ( ore.Count <= 0 )
						{
							ore.Count = 0;
							miningProgressCB.RemoveComponent<MiningWork>( entityInQueryIndex, e );
							miningProgressCB.SetComponent( entityInQueryIndex, e, new WorkProgress() { Progress = 0 } );

							if ( miningTags.Exists( miningWork.Worker ) )
							{
								miningProgressCB.RemoveComponent<MiningTag>( entityInQueryIndex, miningWork.Worker );
								miningProgressCB.AddComponent<IdleTag>( entityInQueryIndex, miningWork.Worker );
							}

							var changeEntity = miningProgressCB.CreateEntity(entityInQueryIndex, changedOreArchetypeLocal);
							miningProgressCB.SetComponent( entityInQueryIndex, changeEntity, new ResourceOreChange() { oreEntity = e } );
						}

						if ( oresMined > 0 )
						{
							var minedEntity = miningProgressCB.CreateEntity( entityInQueryIndex, minedArchetypeLocal );
							miningProgressCB.SetComponent( entityInQueryIndex, minedEntity, new MinedOre() { MinedCount = oresMined, Type = ore.Type } );
							miningProgressCB.SetComponent( entityInQueryIndex, minedEntity, new MapIndex( in index ) );
						}
					}
				} ).ScheduleParallel();

			var ores = GetComponentDataFromEntity<ResourceOre>( true );
			var stocks = GetComponentDataFromEntity<Stock>( true );
			var tiles = mapSettings.Tiles;
			var neighborsLocal = _neighbors;

			// TODO: Enable burst (native queue allocation problem)
			Entities
				.WithReadOnly( neighborsLocal )
				.WithReadOnly( ores )
				.WithReadOnly( stocks )
				.WithReadOnly( tiles )
				.WithNativeDisableContainerSafetyRestriction( tiles )
				.WithoutBurst()
				.ForEach( ( Entity e, int entityInQueryIndex, in MinedOre minedOre, in MapIndex oreIndex ) =>
				 {
					 #region Found stock
					 var stock = new Entity();

					 bool founded = false;
					 var tilesSize = tiles.Length;

					 NativeQueue<int> toSearch = new NativeQueue<int>(Allocator.Temp);
					 NativeArray<Boolean> searched = new NativeArray<Boolean>(tiles.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
					 toSearch.Enqueue( oreIndex.Index1D );

					 bool skipNext = true;
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

						 if ( skipNext )
						 {
							 skipNext = false;
							 continue;
						 }

						 // No ore tile
						 if ( ores.HasComponent( currentEntity ) == false || ores[currentEntity].IsValid == false )
						 {
							 // No stock exists
							 if ( stocks.HasComponent( currentEntity ) == false )
							 {
								 stock = currentEntity;
								 founded = true;
							 }
							 else
							 {
								 var currentStock = stocks[currentEntity];
								 var leftSpace = currentStock.Capacity - currentStock.Count;
								 if ( currentStock.Type == minedOre.Type && leftSpace >= minedOre.MinedCount )
								 {
									 stock = currentEntity;
									 founded = true;
								 }
							 }
						 }
					 }

					 toSearch.Dispose();
					 searched.Dispose();
					 #endregion Found stock

					 if ( founded )
					 {
						 spawnStocksCB.AddComponent( entityInQueryIndex, stock, new StockCountChange() { Count = minedOre.MinedCount, Type = minedOre.Type } );
					 }

					 spawnStocksCB.DestroyEntity( entityInQueryIndex, e );
				 } ).ScheduleParallel();

			_removeCmdBufferSystem.AddJobHandleForProducer( Dependency );
		}

		protected override void OnDestroy() => _neighbors.Dispose();
	}
}