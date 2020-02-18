using Unity.Entities;
using Unity.Jobs;

using Working.Components;

namespace Working.Systems
{
	// TODO: Find A way to use IWork instead of MiningWork Here some ideas https://forum.unity.com/threads/querying-components-on-an-entity-via-interface.663511/
	[UpdateBefore( typeof( MiningWorkSystem ) )]
	public class WorkProgressionSystem : JobComponentSystem
	{
		private EndSimulationEntityCommandBufferSystem _removeCmdBufferSystem;

		protected override void OnCreate() => _removeCmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

		protected override JobHandle OnUpdate( JobHandle inputDeps )
		{
			var cmdBuffer = _removeCmdBufferSystem.CreateCommandBuffer().ToConcurrent();
			var addProgressHandle = Entities
				.WithNone<WorkProgress>()
				.WithAll<MiningWork>()
				.ForEach( ( Entity e, int entityInQueryIndex ) =>
				{
					cmdBuffer.AddComponent<WorkProgress>( entityInQueryIndex, e);
				} ).Schedule( inputDeps );

			var deltaTime = Time.DeltaTime;
			var updateHandle = Entities.ForEach( ( ref WorkProgress workProgress, in MiningWork work ) =>
			{
				workProgress.Progress += work.ProgressPerSecond * deltaTime;
			} ).Schedule( addProgressHandle );

			_removeCmdBufferSystem.AddJobHandleForProducer( addProgressHandle );

			return updateHandle;
		}
	}
}