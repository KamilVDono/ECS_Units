using Maps.Components;
using Maps.Systems;

using Pathfinding.Components;

using Unity.Collections;
using Unity.Entities;

namespace Pathfinding.Systems
{
    [UpdateAfter( typeof( MapSpawner ) )]
    [UpdateInGroup( typeof( InitializationSystemGroup ) )]
    public class MovementCostTrackerSystem : ComponentSystem
    {
        private EntityQuery _changedDataQuery;

        protected override void OnCreate()
        {
            var queryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof( MovementCost ), typeof( GroundType ) },
            };

            _changedDataQuery = GetEntityQuery( queryDesc );
            _changedDataQuery.SetFilterChanged( typeof( GroundType ) );
        }

        protected override void OnUpdate()
        {
            var changed = _changedDataQuery.ToEntityArray( Allocator.TempJob );

            for ( int i = 0; i < changed.Length; i++ )
            {
                var groundCost = EntityManager.GetSharedComponentData<GroundType>( changed[i] );
                EntityManager.SetComponentData( changed[i], new MovementCost { Cost = groundCost.TileTypeBlob.Value.MoveCost } );
            }

            changed.Dispose();
        }
    }
}
