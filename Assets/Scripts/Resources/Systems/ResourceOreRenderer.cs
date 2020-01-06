using Helpers;

using Maps.Components;

using Resources.Components;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

using UnityEngine;

namespace Resources.Systems
{
	internal struct HasResourceOreRenderer : ISystemStateComponentData
	{
		public Entity VisualEntity;
		public Boolean Valid;
	}

	[UpdateInGroup( typeof( PresentationSystemGroup ) )]
	public class ResourceOreRenderer : ComponentSystem
	{
		private EntityArchetype _tileVisualArchetype;
		private Mesh _oreMesh;
		private Material _oreMaterial;

		protected override void OnCreate()
		{
			_tileVisualArchetype = EntityManager.CreateArchetype(
				typeof( RenderMesh ),
				typeof( LocalToWorld ),
				typeof( Translation )
			);

			_oreMesh = MeshCreator.Quad( 0.35f, quaternion.Euler( new float3( math.radians( -90 ), 0, 0 ) ) );

			_oreMaterial = new Material( Shader.Find( "Map/Tile" ) );
			_oreMaterial.SetColor( "_MainColor", new Color32( 20, 20, 20, 255 ) );
		}

		protected override void OnUpdate()
		{
			Entities.WithNone<HasResourceOreRenderer>().ForEach(
				( Entity entity, ref ResourceOre ore, ref MapIndex mapIndex ) =>
				{
					// Need valid entity right now so can not use PostUpdateCommands here
					var visualEntity = new Entity();
					var isValidOre = ore.None == false && ore.Empty == false;
					if ( isValidOre )
					{
						visualEntity = EntityManager.CreateEntity( _tileVisualArchetype );
						PostUpdateCommands.SetSharedComponent( visualEntity, new RenderMesh { mesh = _oreMesh, material = _oreMaterial } );
						PostUpdateCommands.SetComponent( visualEntity, new Translation { Value = new float3( mapIndex.Index2D.x, 0.1f, mapIndex.Index2D.y ) } );
					}

					PostUpdateCommands.AddComponent( entity, new HasResourceOreRenderer { VisualEntity = visualEntity, Valid = isValidOre } );
				} );

			Entities.WithNone<ResourceOre>().ForEach(
				( Entity entity, ref HasResourceOreRenderer visual ) =>
				{
					if ( visual.Valid )
					{
						PostUpdateCommands.DestroyEntity( visual.VisualEntity );
					}
					PostUpdateCommands.RemoveComponent<HasResourceOreRenderer>( entity );
				} );
		}
	}
}