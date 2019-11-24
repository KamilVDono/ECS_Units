using NUnit.Framework;

using PathFinding.Helpers;

using System.Linq;

using Unity.Mathematics;

using static Helpers.IndexUtils;
using static NUnit.Framework.Assert;
using static PathFinding.Helpers.Neighbor;

namespace Tests.Helpers
{
	public class Helpers
	{
		public class EdgeSize
		{
			[Test]
			public void EgdeSize_One()
			{
				int realEgdeSize = 1;
				int mapSize = realEgdeSize * realEgdeSize;

				int edgeSize = EdgeSize(mapSize);

				AreEqual( realEgdeSize, edgeSize );
			}

			[Test]
			public void EgdeSize_Ten()
			{
				int realEgdeSize = 10;
				int mapSize = realEgdeSize * realEgdeSize;

				int edgeSize = EdgeSize(mapSize);

				AreEqual( realEgdeSize, edgeSize );
			}
		}

		public class Index1D
		{
			[Test]
			public void Index1D_Zero_Zero()
			{
				int mapSize = 5 * 5;
				int2 index2D = new int2(0, 0);

				int index = Index1D(index2D, mapSize);

				AreEqual( 0, index );
			}

			[Test]
			public void Index1D_Four_Four()
			{
				int mapSize = 5 * 5;
				int2 index2D = new int2(4, 4);

				int index = Index1D(index2D, mapSize);

				AreEqual( 24, index );
			}

			[Test]
			public void Index1D_Zero_Four()
			{
				int mapSize = 5 * 5;
				int2 index2D = new int2(0, 4);

				int index = Index1D(index2D, mapSize);

				AreEqual( 20, index );
			}

			[Test]
			public void Index1D_Four_Zero()
			{
				int mapSize = 5 * 5;
				int2 index2D = new int2(4, 0);

				int index = Index1D(index2D, mapSize);

				AreEqual( 4, index );
			}

			[Test]
			public void Index1D_Three_Three()
			{
				int mapSize = 5 * 5;
				int2 index2D = new int2(3, 3);

				int index = Index1D(index2D, mapSize);

				AreEqual( 18, index );
			}
		}

		public class Index2D
		{
			[Test]
			public void Index1D_Zero()
			{
				int mapSize = 5 * 5;
				int index1D = 0;

				int2 index = Index2D(index1D, mapSize);

				AreEqual( new int2( 0, 0 ), index );
			}

			[Test]
			public void Index1D_TwentyFour()
			{
				int mapSize = 5 * 5;
				int index1D = 24;

				int2 index = Index2D(index1D, mapSize);

				AreEqual( new int2( 4, 4 ), index );
			}

			[Test]
			public void Index1D_Twenty()
			{
				int mapSize = 5 * 5;
				int index1D = 20;

				int2 index = Index2D(index1D, mapSize);

				AreEqual( new int2( 0, 4 ), index );
			}

			[Test]
			public void Index1D_Four()
			{
				int mapSize = 5 * 5;
				int index1D = 4;

				int2 index = Index2D(index1D, mapSize);

				AreEqual( new int2( 4, 0 ), index );
			}

			[Test]
			public void Index1D_Seventeen()
			{
				int mapSize = 5 * 5;
				int index1D = 17;

				int2 index = Index2D(index1D, mapSize);

				AreEqual( new int2( 2, 3 ), index );
			}
		}

		public class World_To_Index
		{
			private const float epsilon = 0.00001f;

			[Test]
			public void WorldIndex_OneTile_LBCorner_Over()
			{
				var edgeSize = 1;
				var mapSize = edgeSize * edgeSize;
				var worldPosition = new float2(-0.5f, -0.5f) - new float2(epsilon, epsilon);

				Throws( typeof( System.ArgumentOutOfRangeException ), () => WorldIndex1D( worldPosition, mapSize ) );
			}

			[Test]
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
			public void WorldIndex_OneTile_RUCorner_Over()
			{
				var edgeSize = 1;
				var mapSize = edgeSize * edgeSize;
				var worldPosition = new float2(0.5f, 0.5f);

				Throws( typeof( System.ArgumentOutOfRangeException ), () => WorldIndex1D( worldPosition, mapSize ) );
			}

			[Test]
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

		public class Neighbors
		{
			private static readonly Neighbor[] NEIGHBOR = new Neighbor[]{
					UPPER_LEFT, UPPER, UPPER_RIGHT, LEFT, RIGHT, BOTTOM_LEFT, BOTTOM, BOTTOM_RIGHT
				};

			[Test]
			public void Neighbors_Zero()
			{
				int rowSize = 5;
				int mapSize = rowSize * rowSize;
				int index1D = 0;
				int2 index = Index2D(index1D, mapSize);

				int[] exceptedNeighbors = new int[]
			{
				-1, 5, 6, -1, 1, -1, -1, -1,
			};
				var result = NEIGHBOR.Select( n => n.Of( index, mapSize ) ).ToArray();

				AreEqual( exceptedNeighbors, result );
			}

			[Test]
			public void Neighbors_Last()
			{
				int rowSize = 5;
				int mapSize = rowSize * rowSize;
				int index1D = 24;
				int2 index = Index2D(index1D, mapSize);

				int[] exceptedNeighbors = new int[]
			{
				-1, -1, -1, 23, -1, 18, 19, -1
			};
				var result = NEIGHBOR.Select( n => n.Of( index, mapSize ) ).ToArray();

				AreEqual( exceptedNeighbors, result );
			}

			[Test]
			public void Neighbors_Six()
			{
				int rowSize = 5;
				int mapSize = rowSize * rowSize;
				int index1D = 6;
				int2 index = Index2D(index1D, mapSize);

				int[] exceptedNeighbors = new int[]
			{
				10, 11, 12, 5, 7, 0, 1, 2
			};
				var result = NEIGHBOR.Select( n => n.Of( index, mapSize ) ).ToArray();

				AreEqual( exceptedNeighbors, result );
			}

			[Test]
			public void Neighbors_One()
			{
				int rowSize = 5;
				int mapSize = rowSize * rowSize;
				int index1D = 1;
				int2 index = Index2D(index1D, mapSize);

				int[] exceptedNeighbors = new int[]
			{
				5, 6, 7, 0, 2, -1, -1, -1
			};
				var result = NEIGHBOR.Select( n => n.Of( index, mapSize ) ).ToArray();

				AreEqual( exceptedNeighbors, result );
			}

			[Test]
			public void Neighbors_Twenty()
			{
				int rowSize = 5;
				int mapSize = rowSize * rowSize;
				int index1D = 20;
				int2 index = Index2D(index1D, mapSize);

				int[] exceptedNeighbors = new int[]
			{
				-1, -1, -1, -1, 21, -1, 15, 16
			};
				var result = NEIGHBOR.Select( n => n.Of( index, mapSize ) ).ToArray();

				AreEqual( exceptedNeighbors, result );
			}
		}
	}
}