﻿using Pathfinding.Components;

using Unity.Entities;
using Unity.Jobs;

namespace Pathfinding.Systems
{
	public class WaypointCleanupSystem : SystemBase
	{
		private EndSimulationEntityCommandBufferSystem _cmdBufferSystem;

		protected override void OnCreate() => _cmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

		protected override void OnUpdate()
		{
			var cmdBuffer = _cmdBufferSystem.CreateCommandBuffer().ToConcurrent();
			Entities
				.ForEach( ( Entity e, int entityInQueryIndex, in DynamicBuffer<Waypoint> waypoints ) =>
				{
					if ( waypoints.Length < 1 )
					{
						cmdBuffer.RemoveComponent<Waypoint>( entityInQueryIndex, e );
					}
				} ).ScheduleParallel();

			_cmdBufferSystem.AddJobHandleForProducer( Dependency );
		}
	}
}