using System;

using Unity.Burst;
using Unity.Mathematics;

namespace Helpers
{
	public static class IndexUtils
	{
		public static int EdgeSize( int fullSize )
		{
			var edge = (int)math.round( math.sqrt( fullSize ) );
			CheckEdgeSize( edge, fullSize );
			return edge;
		}

		public static int Index1D( int2 index2D, int fullSize ) => Index1DEdge( index2D, EdgeSize( fullSize ) );

		public static int Index1DEdge( int2 index2D, int edgeSize )
		{
			CheckIndex1DEdge( index2D, edgeSize );
			return index2D.y * edgeSize + index2D.x;
		}

		public static int2 Index2D( int index, int fullSize ) => Index2DEdge( index, EdgeSize( fullSize ) );

		public static int2 Index2DEdge( int index, int edgeSize )
		{
			CheckIndex2DEdge( index, edgeSize );
			return new int2( index % edgeSize, index / edgeSize );
		}

		public static int2 WorldIndex2D( float2 position )
		{
			position = position + 0.5f;
			return (int2)math.floor( position );
		}

		public static int WorldIndex1D( float2 position, int fullSize ) => Index1D( WorldIndex2D( position ), fullSize );

		public static int WorldIndex1DEdge( float2 position, int edgeSize ) => Index1DEdge( WorldIndex2D( position ), edgeSize );

		[System.Diagnostics.Conditional( "DEBUG" )]
		[BurstDiscard]
		private static void CheckEdgeSize( int edge, int fullSize )
		{
			if ( fullSize - ( edge * edge ) > 0.001f )
			{
				throw new ArgumentOutOfRangeException( $"In method EdgeSize the inputed size ({nameof( fullSize )}) is not from square world ({fullSize})" );
			}
		}

		[System.Diagnostics.Conditional( "DEBUG" )]
		[BurstDiscard]
		private static void CheckIndex1DEdge( int2 index2D, int edgeSize )
		{
			if ( index2D.x < 0 || index2D.x >= edgeSize || index2D.y < 0 || index2D.y >= edgeSize )
			{
				throw new ArgumentOutOfRangeException( $"In method Index1D the inputed index ({nameof( index2D )}):[{index2D}] is not in world bound ({edgeSize}, {edgeSize})" );
			}
		}

		[System.Diagnostics.Conditional( "DEBUG" )]
		[BurstDiscard]
		private static void CheckIndex2DEdge( int index, int edgeSize )
		{
			if ( index < 0 || index >= edgeSize * edgeSize )
			{
				throw new ArgumentOutOfRangeException( $"In method Index2D the inputed index ({nameof( index )}) is not in world bound ({index})" );
			}
		}
	}
}