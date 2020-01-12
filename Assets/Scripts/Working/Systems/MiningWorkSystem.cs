using Resources.Components;

using Unity.Entities;

using Working.Components;

namespace Working.Systems
{
    [UpdateAfter( typeof( WorkProgressionSystem ) )]
    public class MiningWorkSystem : ComponentSystem
    {
        protected override void OnUpdate() => Entities.WithAll<MiningWork>().ForEach( ( ref WorkProgress workProgress, ref ResourceOre ore ) =>
        {
            if ( ore.IsValid )
            {
                int oresMined = 0;
                while ( workProgress.Progress > ore.Type.Value.WorkRequired )
                {
                    workProgress.Progress -= ore.Type.Value.WorkRequired;
                    oresMined++;
                    ore.Count--;
                }

                if ( ore.Count < 0 )
                {
                    ore.Count = 0;
                }
            }
        } );
    }
}
