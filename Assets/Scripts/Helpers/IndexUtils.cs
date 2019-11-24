using System;

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

		public static int Index1D( int2 index2D, int fullSize )
		{
			CheckIndex1D( index2D, fullSize );
			return index2D.y * EdgeSize( fullSize ) + index2D.x;
		}

		public static int2 Index2D( int index, int fullSize )
		{
			CheckIndex2D( index, fullSize );
			return new int2( index % EdgeSize( fullSize ), index / EdgeSize( fullSize ) );
		}

		public static int WorldIndex1D( float2 position, int fullSize )
		{
			position = position + 0.5f;
			int2 index2 = (int2)math.floor(position);
			return Index1D( index2, fullSize );
		}

		[System.Diagnostics.Conditional( "DEBUG" )]
		private static void CheckEdgeSize( int edge, int fullSize )
		{
			if ( fullSize - ( edge * edge ) > 0.001f )
			{
				throw new ArgumentOutOfRangeException( $"In method EdgeSize the inputed size ({nameof( fullSize )}) is not from square world ({fullSize})" );
			}
		}

		[System.Diagnostics.Conditional( "DEBUG" )]
		private static void CheckIndex1D( int2 index2D, int fullSize )
		{
			var edgeSize = EdgeSize(fullSize);
			if ( index2D.x < 0 || index2D.x >= edgeSize || index2D.y < 0 || index2D.y >= edgeSize )
			{
				throw new ArgumentOutOfRangeException( $"In method Index1D the inputed index ({nameof( index2D )}) is not in world bound ({index2D})" );
			}
		}

		[System.Diagnostics.Conditional( "DEBUG" )]
		private static void CheckIndex2D( int index, int fullSize )
		{
			if ( index < 0 || index >= fullSize )
			{
				throw new ArgumentOutOfRangeException( $"In method Index2D the inputed index ({nameof( index )}) is not in world bound ({index})" );
			}
		}
	}
}