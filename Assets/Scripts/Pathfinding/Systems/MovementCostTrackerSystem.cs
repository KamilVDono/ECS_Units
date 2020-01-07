using Maps.Components;
using Maps.Systems;

using Pathfinding.Components;

using Resources.Components;

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
				All = new ComponentType[] { typeof( MovementCost ), typeof( GroundType ), typeof( ResourceOre ) },
			};

			_changedDataQuery = GetEntityQuery( queryDesc );
			_changedDataQuery.SetChangedVersionFilter( typeof( GroundType ) );
			_changedDataQuery.SetChangedVersionFilter( typeof( ResourceOre ) );
		}

		protected override void OnUpdate()
		{
			var changed = _changedDataQuery.ToEntityArray( Allocator.TempJob );

			for ( int i = 0; i < changed.Length; i++ )
			{
				var groundCost = EntityManager.GetComponentData<GroundType>( changed[i] );
				var resourceOre = EntityManager.GetComponentData<ResourceOre>( changed[i] );
				var resourceOreCost = 0f;
				if ( resourceOre.Empty == false && resourceOre.None == false )
				{
					resourceOreCost = resourceOre.Type.Value.MovementCost;
				}
				EntityManager.SetComponentData( changed[i], new MovementCost { Cost = groundCost.TileTypeBlob.Value.MoveCost + resourceOreCost } );
			}

			changed.Dispose();
		}
	}
}