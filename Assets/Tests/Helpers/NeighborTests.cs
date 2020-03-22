using Helpers.Types;

using NUnit.Framework;

using System;
using System.Linq;

using Tests.Categories;

using Unity.Mathematics;

using static Helpers.IndexUtils;
using static Helpers.Types.Neighbor;
using static NUnit.Framework.Assert;

namespace Tests.Helpers
{
	public class NeighborTests
	{
		private static readonly Neighbor[] NEIGHBOR = new Neighbor[]{
					UPPER_LEFT, UPPER, UPPER_RIGHT, LEFT, RIGHT, BOTTOM_LEFT, BOTTOM, BOTTOM_RIGHT
				};

		[Test]
		[UtilsTest]
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
		[UtilsTest]
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
		[UtilsTest]
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
		[UtilsTest]
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
		[UtilsTest]
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

		[Test]
		[UtilsTest]
		public void Neighbors_Distances()
		{
			float[] exceptedNeighbors = new float[]
			{
				DIAGONAL_DISTANCE, SIMPLE_DISTANCE, DIAGONAL_DISTANCE, SIMPLE_DISTANCE, SIMPLE_DISTANCE, DIAGONAL_DISTANCE, SIMPLE_DISTANCE, DIAGONAL_DISTANCE
			};
			var result = NEIGHBOR.Select( n => n.Distance ).ToArray();

			AreEqual( exceptedNeighbors, result );
		}

		[Test]
		[UtilsTest]
		public void Neighbor_Self() => Throws<ArgumentException>( () => new Neighbor( 0, 0 ) );

		[Test]
		[UtilsTest]
		public void Neighbor_TooDistantHorizontal_Right() => Throws<ArgumentOutOfRangeException>( () => new Neighbor( 2, 0 ) );

		[Test]
		[UtilsTest]
		public void Neighbor_TooDistantHorizontal_Left() => Throws<ArgumentOutOfRangeException>( () => new Neighbor( -2, 0 ) );

		[Test]
		[UtilsTest]
		public void Neighbor_TooDistantVertical_Upper() => Throws<ArgumentOutOfRangeException>( () => new Neighbor( 0, 2 ) );

		[Test]
		[UtilsTest]
		public void Neighbor_TooDistantVertical_Bottom() => Throws<ArgumentOutOfRangeException>( () => new Neighbor( 0, -2 ) );
	}
}