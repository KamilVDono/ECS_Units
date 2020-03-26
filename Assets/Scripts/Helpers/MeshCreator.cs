using System;

using Unity.Mathematics;

using UnityEngine;

namespace Helpers
{
	public static class MeshCreator
	{
		public static Mesh Quad( float extend ) => Quad( extend, quaternion.identity );

		public static Mesh Quad( float extend, quaternion rotation )
		{
			CheckQuadExtend( extend );

			return new Mesh
			{
				vertices = new Vector3[] {
					math.mul(rotation, new float3(-extend, -extend, 0)),
					math.mul(rotation, new float3(-extend,  extend, 0)),
					math.mul(rotation, new float3( extend,  extend, 0)),
					math.mul(rotation, new float3( extend, -extend, 0)),
				},
				triangles = new int[]
				{
					0, 1, 2,
					2, 3, 0,
				},
				uv = new Vector2[]
				{
					new Vector2(0, 0),
					new Vector2(0, 1),
					new Vector2(1, 1),
					new Vector2(1, 0),
				}
			};
		}

		[System.Diagnostics.Conditional( "DEBUG" )]
		private static void CheckQuadExtend( float extend )
		{
			if ( extend <= 0.00001f )
			{
				throw new ArgumentException(
					nameof( extend ),
					$"Parameter {nameof( extend )}cannot both be less or equal zero" );
			}
		}
	}
}