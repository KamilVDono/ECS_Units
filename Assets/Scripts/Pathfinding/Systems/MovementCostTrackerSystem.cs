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
		protected override JobHandle OnUpdate( JobHandle inputDependencies ) =>
			Entities
				//.WithChangeFilter<GroundType>()
				//.WithChangeFilter<ResourceOre>()
				.ForEach( ( ref MovementCost movementCost, in GroundType groundType, in ResourceOre resourceOre ) =>
				{
					var resourceOreCost = 0f;
					if ( resourceOre.IsValid )
					{
						resourceOreCost = resourceOre.Type.Value.MovementCost;
					}

					movementCost.Cost = groundType.TileTypeBlob.Value.MoveCost + resourceOreCost;
				} ).Schedule( inputDependencies );
	}
}