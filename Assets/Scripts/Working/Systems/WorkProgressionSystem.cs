using Unity.Entities;

using Working.Components;

namespace Working.Systems
{
    // TODO: Find A way to use IWork instead of MiningWork
    // Here some ideas https://forum.unity.com/threads/querying-components-on-an-entity-via-interface.663511/
    public class WorkProgressionSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            Entities.ForEach( ( ref MiningWork work, ref WorkProgress workProgress ) =>
            {
                workProgress.Progress += work.ProgressPerSecond * deltaTime;
            } );
        }
    }
}
