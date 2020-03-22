using NUnit.Framework;

using Tests.Categories;

using Unity.Mathematics;

using static Helpers.IndexUtils;
using static NUnit.Framework.Assert;

namespace Tests.Helpers
{
	public class IndexUtilsTests
	{
		public class EdgeSize
		{
			[Test]
			[UtilsTest]
			public void EgdeSize_One()
			{
				int realEgdeSize = 1;
				int mapSize = realEgdeSize * realEgdeSize;

				int edgeSize = EdgeSize(mapSize);

				AreEqual( realEgdeSize, edgeSize );
			}

			[Test]
			[UtilsTest]
			public void EgdeSize_Ten()
			{
				int realEgdeSize = 10;
				int mapSize = realEgdeSize * realEgdeSize;

				int edgeSize = EdgeSize(mapSize);

				AreEqual( realEgdeSize, edgeSize );
			}

			[Test]
			[UtilsTest]
			public void EgdeSize_NoSquareWorld()
			{
				int realEgdeSize = 10;
				int mapSize = realEgdeSize * (realEgdeSize+1);

				Throws( typeof( System.ArgumentOutOfRangeException ), () => EdgeSize( mapSize ) );
			}
		}

		public class Index1D
		{
			[Test]
			[UtilsTest]
			public void Index1D_Zero_Zero()
			{
				int mapSize = 5 * 5;
				int2 index2D = new int2(0, 0);

				int index = Index1D(index2D, mapSize);

				AreEqual( 0, index );
			}

			[Test]
			[UtilsTest]
			public void Index1D_Four_Four()
			{
				int mapSize = 5 * 5;
				int2 index2D = new int2(4, 4);

				int index = Index1D(index2D, mapSize);

				AreEqual( 24, index );
			}

			[Test]
			[UtilsTest]
			public void Index1D_Zero_Four()
			{
				int mapSize = 5 * 5;
				int2 index2D = new int2(0, 4);

				int index = Index1D(index2D, mapSize);

				AreEqual( 20, index );
			}

			[Test]
			[UtilsTest]
			public void Index1D_Four_Zero()
			{
				int mapSize = 5 * 5;
				int2 index2D = new int2(4, 0);

				int index = Index1D(index2D, mapSize);

				AreEqual( 4, index );
			}

			[Test]
			[UtilsTest]
			public void Index1D_Three_Three()
			{
				int mapSize = 5 * 5;
				int2 index2D = new int2(3, 3);

				int index = Index1D(index2D, mapSize);

				AreEqual( 18, index );
			}
		}

		public class Index1DEdge
		{
			[Test]
			[UtilsTest]
			public void Index1DEdge_Zero_Zero()
			{
				int edgeSize = 5;
				int2 index2D = new int2(0, 0);

				int index = Index1DEdge(index2D, edgeSize);

				AreEqual( 0, index );
			}

			[Test]
			[UtilsTest]
			public void Index1DEdge_Four_Four()
			{
				int edgeSize = 5;
				int2 index2D = new int2(4, 4);

				int index = Index1DEdge(index2D, edgeSize);

				AreEqual( 24, index );
			}

			[Test]
			[UtilsTest]
			public void Index1DEdge_Zero_Four()
			{
				int edgeSize = 5;
				int2 index2D = new int2(0, 4);

				int index = Index1DEdge(index2D, edgeSize);

				AreEqual( 20, index );
			}

			[Test]
			[UtilsTest]
			public void Index1DEdge_Four_Zero()
			{
				int edgeSize = 5;
				int2 index2D = new int2(4, 0);

				int index = Index1DEdge(index2D, edgeSize);

				AreEqual( 4, index );
			}

			[Test]
			[UtilsTest]
			public void Index1DEdge_Three_Three()
			{
				int edgeSize = 5;
				int2 index2D = new int2(3, 3);

				int index = Index1DEdge(index2D, edgeSize);

				AreEqual( 18, index );
			}
		}

		public class Index2D
		{
			[Test]
			[UtilsTest]
			public void Index2D_Zero()
			{
				int mapSize = 5 * 5;
				int index1D = 0;

				int2 index = Index2D(index1D, mapSize);

				AreEqual( new int2( 0, 0 ), index );
			}

			[Test]
			[UtilsTest]
			public void Index2D_TwentyFour()
			{
				int mapSize = 5 * 5;
				int index1D = 24;

				int2 index = Index2D(index1D, mapSize);

				AreEqual( new int2( 4, 4 ), index );
			}

			[Test]
			[UtilsTest]
			public void Index2D_Twenty()
			{
				int mapSize = 5 * 5;
				int index1D = 20;

				int2 index = Index2D(index1D, mapSize);

				AreEqual( new int2( 0, 4 ), index );
			}

			[Test]
			[UtilsTest]
			public void Index2D_Four()
			{
				int mapSize = 5 * 5;
				int index1D = 4;

				int2 index = Index2D(index1D, mapSize);

				AreEqual( new int2( 4, 0 ), index );
			}

			[Test]
			[UtilsTest]
			public void Index2D_Seventeen()
			{
				int mapSize = 5 * 5;
				int index1D = 17;

				int2 index = Index2D(index1D, mapSize);

				AreEqual( new int2( 2, 3 ), index );
			}
		}

		public class Index2DEdge
		{
			[Test]
			[UtilsTest]
			public void Index2DEdge_Zero()
			{
				int edgeSize = 5;
				int index1D = 0;

				int2 index = Index2DEdge(index1D, edgeSize);

				AreEqual( new int2( 0, 0 ), index );
			}

			[Test]
			[UtilsTest]
			public void Index2DEdge_TwentyFour()
			{
				int edgeSize = 5;
				int index1D = 24;

				int2 index = Index2DEdge(index1D, edgeSize);

				AreEqual( new int2( 4, 4 ), index );
			}

			[Test]
			[UtilsTest]
			public void Index2DEdge_Twenty()
			{
				int edgeSize = 5;
				int index1D = 20;

				int2 index = Index2DEdge(index1D, edgeSize);

				AreEqual( new int2( 0, 4 ), index );
			}

			[Test]
			[UtilsTest]
			public void Index2DEdge_Four()
			{
				int edgeSize = 5;
				int index1D = 4;

				int2 index = Index2DEdge(index1D, edgeSize);

				AreEqual( new int2( 4, 0 ), index );
			}

			[Test]
			[UtilsTest]
			public void Index2DEdge_Seventeen()
			{
				int edgeSize = 5;
				int index1D = 17;

				int2 index = Index2DEdge(index1D, edgeSize);

				AreEqual( new int2( 2, 3 ), index );
			}

			[Test]
			[UtilsTest]
			public void Index2DEdge_MinusOne()
			{
				int edgeSize = 5;
				int index1D = -1;

				Throws( typeof( System.ArgumentOutOfRangeException ), () => Index2DEdge( index1D, edgeSize ) );
			}

			[Test]
			[UtilsTest]
			public void Index2DEdge_AboveSize()
			{
				int edgeSize = 2;
				int index1D = 4;

				Throws( typeof( System.ArgumentOutOfRangeException ), () => Index2DEdge( index1D, edgeSize ) );
			}
		}

		public class World_To_Index1D
		{
			private const float epsilon = 0.00001f;

			[Test]
			[UtilsTest]
			public void WorldIndex_OneTile_LBCorner_Over()
			{
				var edgeSize = 1;
				var mapSize = edgeSize * edgeSize;
				var worldPosition = new float2(-0.5f, -0.5f) - new float2(epsilon, epsilon);

				Throws( typeof( System.ArgumentOutOfRangeException ), () => WorldIndex1D( worldPosition, mapSize ) );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_OneTile_LBCorner()
			{
				var edgeSize = 1;
				var mapSize = edgeSize * edgeSize;
				var worldPosition = new float2(-0.5f, -0.5f);

				var expected = 0;
				var index = WorldIndex1D( worldPosition, mapSize );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_OneTile_RUCorner_Over()
			{
				var edgeSize = 1;
				var mapSize = edgeSize * edgeSize;
				var worldPosition = new float2(0.5f, 0.5f);

				Throws( typeof( System.ArgumentOutOfRangeException ), () => WorldIndex1D( worldPosition, mapSize ) );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_OneTile_RUCorner()
			{
				var edgeSize = 1;
				var mapSize = edgeSize * edgeSize;
				var worldPosition = new float2(0.5f, 0.5f) - new float2(epsilon, epsilon);

				var expected = 0;
				var index = WorldIndex1D( worldPosition, mapSize );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_OneTile_Center()
			{
				var edgeSize = 1;
				var mapSize = edgeSize * edgeSize;
				var worldPosition = new float2(0, 0);

				var expected = 0;
				var index = WorldIndex1D( worldPosition, mapSize );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_FiveTile_LBCorner()
			{
				var edgeSize = 5;
				var mapSize = edgeSize * edgeSize;
				var worldPosition = new float2(-0.5f, -0.5f);

				var expected = 0;
				var index = WorldIndex1D( worldPosition, mapSize );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_FiveTile_RUCorner()
			{
				var edgeSize = 5;
				var mapSize = edgeSize * edgeSize;
				var worldPosition = new float2(4.5f, 4.5f) - new float2(epsilon, epsilon);

				var expected = 24;
				var index = WorldIndex1D( worldPosition, mapSize );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_FiveTile_Center()
			{
				var edgeSize = 5;
				var mapSize = edgeSize * edgeSize;
				var worldPosition = new float2(2, 2);

				var expected = 12;
				var index = WorldIndex1D( worldPosition, mapSize );

				AreEqual( expected, index );
			}
		}

		public class World_To_Index1D_Edge
		{
			private const float epsilon = 0.00001f;

			[Test]
			[UtilsTest]
			public void WorldIndex_OneTile_LBCorner_Over()
			{
				var edgeSize = 1;
				var worldPosition = new float2(-0.5f, -0.5f) - new float2(epsilon, epsilon);

				Throws( typeof( System.ArgumentOutOfRangeException ), () => WorldIndex1DEdge( worldPosition, edgeSize ) );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_OneTile_LBCorner()
			{
				var edgeSize = 1;
				var worldPosition = new float2(-0.5f, -0.5f);

				var expected = 0;
				var index = WorldIndex1DEdge( worldPosition, edgeSize );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_OneTile_RUCorner_Over()
			{
				var edgeSize = 1;
				var worldPosition = new float2(0.5f, 0.5f);

				Throws( typeof( System.ArgumentOutOfRangeException ), () => WorldIndex1DEdge( worldPosition, edgeSize ) );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_OneTile_RUCorner()
			{
				var edgeSize = 1;
				var worldPosition = new float2(0.5f, 0.5f) - new float2(epsilon, epsilon);

				var expected = 0;
				var index = WorldIndex1DEdge( worldPosition, edgeSize );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_OneTile_Center()
			{
				var edgeSize = 1;
				var worldPosition = new float2(0, 0);

				var expected = 0;
				var index = WorldIndex1DEdge( worldPosition, edgeSize );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_FiveTile_LBCorner()
			{
				var edgeSize = 5;
				var worldPosition = new float2(-0.5f, -0.5f);

				var expected = 0;
				var index = WorldIndex1DEdge( worldPosition, edgeSize );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_FiveTile_RUCorner()
			{
				var edgeSize = 5;
				var worldPosition = new float2(4.5f, 4.5f) - new float2(epsilon, epsilon);

				var expected = 24;
				var index = WorldIndex1DEdge( worldPosition, edgeSize );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_FiveTile_Center()
			{
				var edgeSize = 5;
				var worldPosition = new float2(2, 2);

				var expected = 12;
				var index = WorldIndex1DEdge( worldPosition, edgeSize );

				AreEqual( expected, index );
			}
		}

		public class World_To_Index2D
		{
			private const float epsilon = 0.00001f;

			[Test]
			[UtilsTest]
			public void WorldIndex_OneTile_LBCorner()
			{
				var worldPosition = new float2(-0.5f, -0.5f);

				var expected = new int2(0, 0);
				var index = WorldIndex2D( worldPosition );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_OneTile_RUCorner()
			{
				var worldPosition = new float2(0.5f, 0.5f) - new float2(epsilon, epsilon);

				var expected = new int2(0, 0);
				var index = WorldIndex2D( worldPosition );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_OneTile_Center()
			{
				var worldPosition = new float2(0, 0);

				var expected = new int2(0, 0);
				var index = WorldIndex2D( worldPosition );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_FiveTile_LBCorner()
			{
				var worldPosition = new float2(-0.5f, -0.5f);

				var expected = new int2(0, 0);
				var index = WorldIndex2D( worldPosition );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_FiveTile_RUCorner()
			{
				var worldPosition = new float2(4.5f, 4.5f) - new float2(epsilon, epsilon);

				var expected = new int2(4, 4);
				var index = WorldIndex2D( worldPosition );

				AreEqual( expected, index );
			}

			[Test]
			[UtilsTest]
			public void WorldIndex_FiveTile_Center()
			{
				var worldPosition = new float2(2, 2);

				var expected = new int2(2, 2);
				var index = WorldIndex2D( worldPosition );

				AreEqual( expected, index );
			}
		}
	}
}