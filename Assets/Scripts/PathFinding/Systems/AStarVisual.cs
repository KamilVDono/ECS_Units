using Helpers;

using Input.Systems;

using Pathfinding.Components;

using Rendering.Components;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

using UnityEngine;

namespace Pathfinding.Systems
{
	[UpdateBefore( typeof( AStar ) )]
	public class AStarVisual : SystemBase
	{
		private const float EXTEND = 0.25f;

		private Material _material;
		private Mesh _mesh;

		private EntityArchetype _pathTileArchetype;
		private Unity.Mathematics.Random _random;

		private EndInitializationEntityCommandBufferSystem _cmdBufferSystem;

		protected override void OnCreate()
		{
			_mesh = MeshCreator.Quad( EXTEND, quaternion.Euler( new float3( math.radians( 90 ), 0, 0 ) ) );

			_pathTileArchetype = EntityManager.CreateArchetype(
				typeof( RenderMesh ),
				typeof( LocalToWorld ),
				typeof( Translation ),
				typeof( RenderBounds ),
				typeof( MainColorMaterialProperty )
				);

			_random = new Unity.Mathematics.Random();
			_random.InitState();

			_material = new Material( Shader.Find( "Tile/AStarVisual" ) )
			{
				enableInstancing = true
			};

			_cmdBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
		}

		protected override void OnUpdate()
		{
			var commandBuffer = _cmdBufferSystem.CreateCommandBuffer();

			Entities
				.WithAll<MouseAStarSystem.MouseRequestedPath>()
				.WithoutBurst()
				.WithStructuralChanges()
				.ForEach( ( Entity e, in DynamicBuffer<Waypoint> waypoints ) =>
				{
					var materialColor = new float4( _random.NextFloat(), _random.NextFloat(), _random.NextFloat(), 1 );

					var length = waypoints.Length;

					for ( int x = 0; x < length; x++ )
					{
						var tileEntity = commandBuffer.CreateEntity( _pathTileArchetype );
						var waypoint = waypoints[x];
						commandBuffer.SetSharedComponent( tileEntity, new RenderMesh { mesh = _mesh, material = _material } );
						commandBuffer.SetComponent( tileEntity, new Translation { Value = new float3( waypoint.Position.x, 1, waypoint.Position.y ) } );
						commandBuffer.SetComponent( tileEntity, new RenderBounds
						{
							Value = new AABB()
							{
								Center = float3.zero,
								Extents = new float3( EXTEND, 0, EXTEND )
							}
						} );
						commandBuffer.SetComponent( tileEntity, new MainColorMaterialProperty
						{
							Color = materialColor
						} );
					}

					EntityManager.DestroyEntity( e );
				} ).Run();

			_cmdBufferSystem.AddJobHandleForProducer( Dependency );
		}
	}
}