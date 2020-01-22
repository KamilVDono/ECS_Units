using Unity.Entities;
using Unity.Jobs;

using Working.Components;

namespace Working.Systems
{
	// TODO: Find A way to use IWork instead of MiningWork Here some ideas https://forum.unity.com/threads/querying-components-on-an-entity-via-interface.663511/
	public class WorkProgressionSystem : JobComponentSystem
	{
		protected override JobHandle OnUpdate( JobHandle inputDeps )
		{
			var deltaTime = Time.DeltaTime;
			return Entities.ForEach( ( ref WorkProgress workProgress, in MiningWork work ) =>
			{
				workProgress.Progress += work.ProgressPerSecond * deltaTime;
			} ).Schedule( inputDeps );
		}
	}
}