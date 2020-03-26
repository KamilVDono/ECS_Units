using NUnit.Framework;

using System;

using Tests.Categories;

using Unity.Mathematics;

using UnityEngine;

using static Helpers.MeshCreator;
using static NUnit.Framework.Assert;

namespace Tests.Helpers
{
	public class MeshCreatorTests
	{
		protected static int IndexOf( Vector3[] vectors, Vector3 search )
		{
			for ( int i = 0; i < vectors.Length; i++ )
			{
				if ( vectors[i] == search )
				{
					return i;
				}
			}
			return -1;
		}

		public class Quad_Tests
		{
			[Test]
			[UtilsTest]
			public void Quad_NegativeSize()
			{
				float extend = -1f;
				Throws<ArgumentException>( () => Quad( extend ) );
			}

			[Test]
			[UtilsTest]
			public void Quad_ZeroSize()
			{
				float extend = 0f;
				Throws<ArgumentException>( () => Quad( extend ) );
			}

			[Test]
			[UtilsTest]
			public void SimpleQuad()
			{
				float extend = 1f;
				var quad = Quad(extend);

				NotNull( quad );

				Vector2[] uvs = quad.uv;
				Vector3[] verices = quad.vertices;

				Vector3[] expectedVertices = new Vector3[]{
					new Vector3(-extend, -extend, 0),
					new Vector3(extend, -extend, 0),
					new Vector3(-extend, extend, 0),
					new Vector3(extend, extend, 0),
				};
				Vector2[] expectedUVs = new Vector2[]{
					new Vector2(0, 0),
					new Vector2(1, 0),
					new Vector2(0, 1),
					new Vector2(1, 1),
				};

				for ( int i = 0; i < expectedVertices.Length; i++ )
				{
					int indexOnVertices = IndexOf( verices, expectedVertices[i] );
					True( indexOnVertices >= 0 && indexOnVertices < expectedVertices.Length );
					AreEqual( uvs[indexOnVertices], expectedUVs[i] );
				}
			}

			[Test]
			[UtilsTest]
			public void Quad_Rotation_X()
			{
				float extend = 1f;
				var quad = Quad(extend, quaternion.Euler(math.radians( 90 ), 0, 0));

				Vector3[] verices = quad.vertices;

				Vector3[] expectedVertices = new Vector3[]{
					new Vector3(-extend,0,  -extend),
					new Vector3(extend, 0, -extend),
					new Vector3(-extend,0,  extend),
					new Vector3(extend, 0, extend),
				};

				for ( int i = 0; i < expectedVertices.Length; i++ )
				{
					int indexOnVertices = IndexOf( verices, expectedVertices[i] );
					True( indexOnVertices >= 0 && indexOnVertices < expectedVertices.Length );
				}
			}

			[Test]
			[UtilsTest]
			public void Quad_Rotation_Y()
			{
				float extend = 1f;
				var quad = Quad(extend, quaternion.Euler(0, math.radians( 90 ), 0));

				Vector3[] verices = quad.vertices;

				Vector3[] expectedVertices = new Vector3[]{
					new Vector3(0, -extend, -extend),
					new Vector3(0, extend, -extend),
					new Vector3(0, -extend, extend),
					new Vector3(0, extend, extend),
				};

				for ( int i = 0; i < expectedVertices.Length; i++ )
				{
					int indexOnVertices = IndexOf( verices, expectedVertices[i] );
					True( indexOnVertices >= 0 && indexOnVertices < expectedVertices.Length );
				}
			}

			[Test]
			[UtilsTest]
			public void Quad_Rotation_Z()
			{
				float extend = 1f;
				var quad = Quad(extend, quaternion.Euler(0, 0, math.radians( 90 )));

				Vector3[] verices = quad.vertices;

				Vector3[] expectedVertices = new Vector3[]{
					new Vector3(-extend, -extend, 0),
					new Vector3(extend, -extend, 0),
					new Vector3(-extend, extend, 0),
					new Vector3(extend, extend, 0),
				};

				for ( int i = 0; i < expectedVertices.Length; i++ )
				{
					int indexOnVertices = IndexOf( verices, expectedVertices[i] );
					True( indexOnVertices >= 0 && indexOnVertices < expectedVertices.Length );
				}
			}
		}
	}
}