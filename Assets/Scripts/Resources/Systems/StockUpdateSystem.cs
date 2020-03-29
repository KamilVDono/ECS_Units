using Resources.Components;

using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Resources.Systems
{
	public class StockUpdateSystem : SystemBase
	{
		private const int DEFAULT_STOCK_CAPACITY = 5000;

		private EndSimulationEntityCommandBufferSystem _removeCmdBufferSystem;

		protected override void OnCreate() => _removeCmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

		// TODO: More checks (types), respawn some kind of stock request
		protected override void OnUpdate()
		{
			var stockUpdateCB = _removeCmdBufferSystem.CreateCommandBuffer().ToConcurrent();
			var stockCreateCB = _removeCmdBufferSystem.CreateCommandBuffer().ToConcurrent();

			Entities
				.ForEach( ( Entity e, int entityInQueryIndex, ref Stock stock, ref StockCountChange stockCountChange ) =>
				 {
					 var change = math.min(stockCountChange.Count, stock.AvailableSpace);
					 stock.Count += change;

					 stockUpdateCB.RemoveComponent<StockCountChange>( entityInQueryIndex, e );
				 } ).ScheduleParallel();

			Entities
				.WithNone<Stock>()
				.ForEach( ( Entity e, int entityInQueryIndex, ref StockCountChange stockCountChange ) =>
				{
					stockCreateCB.AddComponent( entityInQueryIndex, e, new Stock()
					{
						Capacity = DEFAULT_STOCK_CAPACITY,
						Count = stockCountChange.Count,
						Type = stockCountChange.Type
					} );
					stockCreateCB.RemoveComponent<StockCountChange>( entityInQueryIndex, e );
				} ).ScheduleParallel();

			_removeCmdBufferSystem.AddJobHandleForProducer( Dependency );
		}
	}
}