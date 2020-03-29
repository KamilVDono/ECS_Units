using Unity.Entities;
using Unity.Jobs;

using Working.Components;

namespace Working.Systems
{
	// TODO: Find A way to use IWork instead of MiningWork Here some ideas https://forum.unity.com/threads/querying-components-on-an-entity-via-interface.663511/
	[UpdateBefore( typeof( MiningWorkSystem ) )]
	public class WorkProgressionSystem : SystemBase
	{
		private EndSimulationEntityCommandBufferSystem _removeCmdBufferSystem;

		protected override void OnCreate() => _removeCmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

		protected override void OnUpdate()
		{
			var cmdBuffer = _removeCmdBufferSystem.CreateCommandBuffer().ToConcurrent();
			Entities
				.WithNone<WorkProgress>()
				.WithAll<MiningWork>()
				.ForEach( ( Entity e, int entityInQueryIndex ) =>
				{
					cmdBuffer.AddComponent<WorkProgress>( entityInQueryIndex, e );
				} ).ScheduleParallel();

			var deltaTime = Time.DeltaTime;
			Entities
				.ForEach( ( ref WorkProgress workProgress, in MiningWork work ) =>
			{
				workProgress.Progress += work.ProgressPerSecond * deltaTime;
			} ).ScheduleParallel();

			_removeCmdBufferSystem.AddJobHandleForProducer( Dependency );
		}
	}
}