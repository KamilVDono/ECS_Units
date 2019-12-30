using Maps.Components;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Maps.Systems
{
    [UpdateInGroup( typeof( PresentationSystemGroup ) )]

    public class GroundRenderSystem : ComponentSystem
    {
        private EntityArchetype _tileVisualArchetype;
        private readonly quaternion _rotation = quaternion.Euler( new float3( math.radians(90), 0, 0 ) );

        protected override void OnCreate() =>
            _tileVisualArchetype = EntityManager.CreateArchetype(
                // Visual
                typeof( RenderMesh ),
                typeof( Rotation ),
                typeof( LocalToWorld ),
                typeof( Translation )
            );

        protected override void OnUpdate()
        {
            Entities.WithNone<HasGroundRenderer>().ForEach(
                ( Entity entity, GroundType groundType, ref MapIndex mapIndex ) =>
                {
                    // Need valid entity right now so can not use PostUpdateCommands here
                    var visualEntity = EntityManager.CreateEntity(_tileVisualArchetype);
                    PostUpdateCommands.SetSharedComponent( visualEntity,
                        new RenderMesh { mesh = groundType.TileTypeBlob.Value.Mesh, material = groundType.TileTypeBlob.Value.Material }
                        );
                    PostUpdateCommands.SetComponent( visualEntity, new Translation { Value = new float3( mapIndex.Index2D.x, 0, mapIndex.Index2D.y ) } );
                    PostUpdateCommands.SetComponent( visualEntity, new Rotation { Value = _rotation } );

                    PostUpdateCommands.AddComponent( entity, new HasGroundRenderer { VisualEntity = visualEntity } );
                } );

            Entities.WithNone<GroundType>().ForEach(
                ( Entity entity, ref HasGroundRenderer visual ) =>
                {
                    PostUpdateCommands.DestroyEntity( visual.VisualEntity );
                    PostUpdateCommands.RemoveComponent<HasGroundRenderer>( entity );
                } );
        }
    }

    internal struct HasGroundRenderer : ISystemStateComponentData
    {
        public Entity VisualEntity;
    }
}
