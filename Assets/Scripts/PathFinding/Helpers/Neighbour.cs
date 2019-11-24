using System;

using Unity.Mathematics;

using static Helpers.IndexUtils;

namespace PathFinding.Helpers
{
	public readonly struct Neighbor
	{
		#region Declared

		public static readonly Neighbor UPPER_LEFT  = new Neighbor( -1, 1 );
		public static readonly Neighbor UPPER       = new Neighbor(  0, 1 );
		public static readonly Neighbor UPPER_RIGHT = new Neighbor(  1, 1 );

		public static readonly Neighbor LEFT        = new Neighbor( -1, 0 );
		public static readonly Neighbor RIGHT       = new Neighbor(  1, 0 );

		public static readonly Neighbor BOTTOM_LEFT  = new Neighbor( -1, -1 );
		public static readonly Neighbor BOTTOM       = new Neighbor(  0, -1 );
		public static readonly Neighbor BOTTOM_RIGHT = new Neighbor(  1, -1 );

		#endregion Declared

		public readonly float Distance;
		public readonly int2 Offset;

		public Neighbor( int x, int y )
		{
			this.Offset = new int2( x, y );
			this.Distance = x != 0 && y != 0 ? 1.41421f : 1;

			CheckValues( x, y );
		}

		public int Of( int index1D, int fullSize ) => Of( Index2D( index1D, fullSize ), fullSize );

		public int Of( int2 index2D, int fullSize )
		{
			var edgeSize = EdgeSize(fullSize);

			index2D += Offset;
			if ( index2D.x < 0 || index2D.x >= edgeSize || index2D.y < 0 || index2D.y >= edgeSize )
			{
				return -1;
			}
			return Index1D( index2D, fullSize );
		}

		[System.Diagnostics.Conditional( "DEBUG" )]
		private void CheckValues( int x, int y )
		{
			if ( x < -1 || x > 1 )
			{
				throw new ArgumentOutOfRangeException(
					nameof( x ),
					$"Parameter {nameof( x )} cannot have a magnitude larger than one" );
			}

			if ( y < -1 || y > 1 )
			{
				throw new ArgumentOutOfRangeException(
					nameof( y ),
					$"Parameter {nameof( y )} cannot have a magnitude larger than one" );
			}

			if ( x == 0 && y == 0 )
			{
				throw new ArgumentException(
					nameof( y ),
					$"Parameters {nameof( x )} and {nameof( y )} cannot both be zero" );
			}
		}
	}
}