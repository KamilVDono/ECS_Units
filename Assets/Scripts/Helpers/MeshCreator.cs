using Unity.Mathematics;

using UnityEngine;

namespace Helpers
{
	public static class MeshCreator
	{
		public static Mesh Quad( float extend ) => Quad( extend, quaternion.identity );

		public static Mesh Quad( float extend, quaternion rotation ) => new Mesh
		{
			vertices = new Vector3[] {
					math.mul(rotation, new float3(-extend, -extend, 0)),
					math.mul(rotation, new float3( extend, -extend, 0)),
					math.mul(rotation, new float3(-extend,  extend, 0)),
					math.mul(rotation, new float3( extend,  extend, 0)),
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
	}
}
