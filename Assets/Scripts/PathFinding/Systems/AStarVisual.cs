using Pathfinding.Components;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

using UnityEngine;
using UnityEngine.Profiling;

namespace Pathfinding.Systems
{
	[UpdateBefore( typeof( AStar ) )]
	public class AStarVisual : ComponentSystem
	{
		private const float EXTEND = 0.25f;

		private static readonly int BASE_COLOR = Shader.PropertyToID("_BaseColor");
		private Material _material;
		private Mesh _mesh;
		private Shader _shader;
		private EntityArchetype _pathTileArchetype;
		private Unity.Mathematics.Random _random;

		protected override void OnCreate()
		{
			_mesh = new Mesh
			{
				vertices = new Vector3[] {
					new Vector3(-EXTEND, -EXTEND),
					new Vector3( EXTEND, -EXTEND),
					new Vector3(-EXTEND,  EXTEND),
					new Vector3( EXTEND,  EXTEND),
				},
				triangles = new int[]
				{
					0, 1, 2,
					1, 3, 2,
				},
				uv = new Vector2[]
				{
					new Vector2(0, 0),
					new Vector2(1, 0),
					new Vector2(0, 1),
					new Vector2(1, 1),
				}
			};

			_shader = Shader.Find( "Tile/AStarVisual" );

			_pathTileArchetype = EntityManager.CreateArchetype(
				typeof( RenderMesh ),
				typeof( LocalToWorld ),
				typeof( Translation ),
				typeof( Rotation )
				);

			_random = new Unity.Mathematics.Random();
			_random.InitState();
		}

		protected override void OnUpdate() => Entities.ForEach( ( Entity e, ref PathRequest p ) =>
		{
			if ( p.Done )
			{
				Profiler.BeginSample( "Spawning-Path_Visual" );
				_material = new Material( _shader );
				var materialColor = new Color( _random.NextFloat(), _random.NextFloat(), _random.NextFloat(), 1 );
				_material.SetColor( BASE_COLOR, materialColor );

				var rotation = quaternion.Euler( new float3( math.radians(-90), 0, 0 ) );

				var buffer = EntityManager.GetBuffer<Waypoint>( e );
				var length = buffer.Length;

				for ( int x = 0; x < length; x++ )
				{
					var tileEntity = PostUpdateCommands.CreateEntity( _pathTileArchetype );
					var waypoint = buffer[x];
					PostUpdateCommands.SetSharedComponent( tileEntity, new RenderMesh { mesh = _mesh, material = _material } );
					PostUpdateCommands.SetComponent( tileEntity, new Translation { Value = new float3( waypoint.Position.x, 1, waypoint.Position.y ) } );
					PostUpdateCommands.SetComponent( tileEntity, new Rotation { Value = rotation } );
				}

				buffer.Clear();
				PostUpdateCommands.DestroyEntity( e );
				Profiler.EndSample();
			}
		} );
	}
}