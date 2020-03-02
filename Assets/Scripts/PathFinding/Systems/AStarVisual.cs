using Helpers;

using Input.Systems;

using Pathfinding.Components;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

using UnityEngine;

namespace Pathfinding.Systems
{
	[UpdateBefore( typeof( AStar ) )]
	public class AStarVisual : ComponentSystem
	{
		private const float EXTEND = 0.25f;

		private static readonly int BASE_COLOR = Shader.PropertyToID("_BaseColor");

		private Material _material;
		private Shader _shader;
		private Mesh _mesh;

		private EntityArchetype _pathTileArchetype;
		private Unity.Mathematics.Random _random;

		protected override void OnCreate()
		{
			_mesh = MeshCreator.Quad( EXTEND, quaternion.Euler( new float3( math.radians( -90 ), 0, 0 ) ) );

			_shader = Shader.Find( "Tile/AStarVisual" );

			_pathTileArchetype = EntityManager.CreateArchetype(
				typeof( RenderMesh ),
				typeof( LocalToWorld ),
				typeof( Translation ),
				typeof( RenderBounds ),
				typeof( WorldRenderBounds ),
				typeof( PerInstanceCullingTag )
				);

			_random = new Unity.Mathematics.Random();
			_random.InitState();
		}

		protected override void OnUpdate()
		{
			Entities
				.WithAll<MouseAStarSystem.MouseRequestedPath>()
				.ForEach( ( Entity e, DynamicBuffer<Waypoint> waypoints ) =>
				{
					_material = new Material( _shader );
					var materialColor = new Color( _random.NextFloat(), _random.NextFloat(), _random.NextFloat(), 1 );
					_material.SetColor( BASE_COLOR, materialColor );
					_material.enableInstancing = true;

					var length = waypoints.Length;

					for ( int x = 0; x < length; x++ )
					{
						var tileEntity = PostUpdateCommands.CreateEntity( _pathTileArchetype );
						var waypoint = waypoints[x];
						PostUpdateCommands.SetSharedComponent( tileEntity, new RenderMesh { mesh = _mesh, material = _material } );
						PostUpdateCommands.SetComponent( tileEntity, new Translation { Value = new float3( waypoint.Position.x, 1, waypoint.Position.y ) } );
						PostUpdateCommands.SetComponent( tileEntity, new RenderBounds
						{
							Value = new AABB()
							{
								Center = float3.zero,
								Extents = new float3( EXTEND, 0, EXTEND )
							}
						} );
					}

					waypoints.Clear();
					PostUpdateCommands.DestroyEntity( e );
				} );
		}
	}
}