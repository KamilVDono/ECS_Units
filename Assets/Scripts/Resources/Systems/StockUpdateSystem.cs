using Resources.Components;

using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Resources.Systems
{
    public class StockUpdateSystem : JobComponentSystem
    {
        private const int DEFAULT_STOCK_CAPACITY = 5000;

        private EndSimulationEntityCommandBufferSystem _removeCmdBufferSystem;

        protected override void OnCreate()
        {
            _removeCmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        // TODO: More checks (types), respawn some kind of stock request
        protected override JobHandle OnUpdate( JobHandle inputDeps )
        {
            var stockUpdateCB = _removeCmdBufferSystem.CreateCommandBuffer().ToConcurrent();
            var stockCreateCB = _removeCmdBufferSystem.CreateCommandBuffer().ToConcurrent();

            var stockUpdateHandle = Entities
                .ForEach( ( Entity e, int entityInQueryIndex, ref Stock stock, ref StockCountChange stockCountChange ) =>
                 {
                     stock.Count += stockCountChange.Count;
                     stock.Count = math.clamp(stock.Count, 0, stock.Capacity);

                     stockUpdateCB.RemoveComponent<StockCountChange>(entityInQueryIndex, e);
                 } ).Schedule( inputDeps );

            var stockCreateHandle = Entities
                .WithNone<Stock>()
                .ForEach( ( Entity e, int entityInQueryIndex, ref StockCountChange stockCountChange ) =>
                {
                    stockCreateCB.AddComponent(entityInQueryIndex, e, new Stock()
                    {
                        Capacity = DEFAULT_STOCK_CAPACITY,
                        Count = stockCountChange.Count,
                        Type = stockCountChange.Type
                    });
                    stockUpdateCB.RemoveComponent<StockCountChange>(entityInQueryIndex, e);
                } ).Schedule( stockUpdateHandle );

            _removeCmdBufferSystem.AddJobHandleForProducer( stockUpdateHandle );
            _removeCmdBufferSystem.AddJobHandleForProducer( stockCreateHandle );

            return stockCreateHandle;
        }
    }
}
