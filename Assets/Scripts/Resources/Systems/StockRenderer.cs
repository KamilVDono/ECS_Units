using Helpers;

using Maps.Components;

using Resources.Components;

using System.Collections.Generic;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

using UnityEngine;

namespace Resources.Systems
{
	internal struct HasStockRenderer : ISystemStateComponentData
	{
		public Entity VisualEntity;
	}

	[UpdateInGroup( typeof( SimulationSystemGroup ) )]
	public class StockRenderer : ComponentSystem
	{
		private EntityArchetype _tileVisualArchetype;
		private Mesh _mesh;
		private Dictionary<Color32, Material> _materials = new Dictionary<Color32, Material>();

		protected override void OnCreate()
		{
			_tileVisualArchetype = EntityManager.CreateArchetype(
				typeof( RenderMesh ),
				typeof( LocalToWorld ),
				typeof( Translation )
			);

			_mesh = MeshCreator.Quad( 0.25f, quaternion.Euler( new float3( math.radians( -90 ), 0, 0 ) ) );
		}

		protected override void OnUpdate()
		{
			Entities.WithNone<HasStockRenderer>().ForEach(
				( Entity entity, ref Stock stock, ref MapIndex mapIndex ) =>
				{
					var visualEntity = PostUpdateCommands.CreateEntity( _tileVisualArchetype );
					PostUpdateCommands.SetSharedComponent( visualEntity, new RenderMesh { mesh = _mesh, material = GetMaterial( stock.Type.Value.Color ) } );
					PostUpdateCommands.SetComponent( visualEntity, new Translation { Value = new float3( mapIndex.Index2D.x, 0.1f, mapIndex.Index2D.y ) } );

					PostUpdateCommands.AddComponent( entity, new HasStockRenderer { VisualEntity = visualEntity } );
				} );

			Entities.WithNone<Stock>().ForEach(
				( Entity entity, ref HasStockRenderer visual ) =>
				{
					PostUpdateCommands.RemoveComponent<HasResourceOreRenderer>( entity );
					PostUpdateCommands.DestroyEntity( visual.VisualEntity );
				} );
		}

		private Material GetMaterial( Color32 color )
		{
			if ( _materials.TryGetValue( color, out var material ) )
			{
				return material;
			}
			material = new Material( Shader.Find( "Map/TileWave" ) );
			material.SetColor( "_MainColor", color );
			_materials.Add( color, material );
			return material;
		}
	}
}

