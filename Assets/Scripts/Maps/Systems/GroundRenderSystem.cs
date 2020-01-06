using Helpers;

using Maps.Components;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

using UnityEngine;

namespace Maps.Systems
{
	internal struct HasGroundRenderer : ISystemStateComponentData
	{
		public Entity VisualEntity;
	}

	[UpdateInGroup( typeof( PresentationSystemGroup ) )]
	public class GroundRenderSystem : ComponentSystem
	{
		private const float EXTEND= 0.5f;

		private EntityArchetype _tileVisualArchetype;
		private Mesh _groundMesh;

		protected override void OnCreate()
		{
			_tileVisualArchetype = EntityManager.CreateArchetype(
				typeof( RenderMesh ),
				typeof( LocalToWorld ),
				typeof( Translation )
			);

			_groundMesh = MeshCreator.Quad( EXTEND, quaternion.Euler( new float3( math.radians( -90 ), 0, 0 ) ) );
		}

		protected override void OnUpdate()
		{
			Entities.WithNone<HasGroundRenderer>().ForEach(
				( Entity entity, ref GroundType groundType, ref MapIndex mapIndex ) =>
				{
					// Need valid entity right now so can not use PostUpdateCommands here
					var visualEntity = EntityManager.CreateEntity(_tileVisualArchetype);
					PostUpdateCommands.SetSharedComponent( visualEntity,
						new RenderMesh { mesh = _groundMesh, material = groundType.TileTypeBlob.Value.Material }
						);
					PostUpdateCommands.SetComponent( visualEntity, new Translation { Value = new float3( mapIndex.Index2D.x, 0, mapIndex.Index2D.y ) } );

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
}