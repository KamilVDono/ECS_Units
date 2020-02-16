using Pathfinding.Components;

using Unity.Entities;
using Unity.Jobs;

namespace Pathfinding.Systems
{
	public class WaypointCleanupSystem : JobComponentSystem
	{
		private EndSimulationEntityCommandBufferSystem _cmdBufferSystem;

		protected override void OnCreate() => _cmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

		protected override JobHandle OnUpdate( JobHandle inputDeps )
		{
			var cmdBuffer = _cmdBufferSystem.CreateCommandBuffer().ToConcurrent();
			var handle = Entities
				.ForEach( ( Entity e, int entityInQueryIndex, in DynamicBuffer<Waypoint> waypoints ) =>
				{
					if(waypoints.Length < 1)
					{
						cmdBuffer.RemoveComponent<Waypoint>(entityInQueryIndex, e);
					}
				} ).Schedule( inputDeps );

			_cmdBufferSystem.AddJobHandleForProducer( handle );
			return handle;
		}
	}
}