﻿using System;

using Unity.Collections;
using Unity.Mathematics;

using static Helpers.IndexUtils;

namespace Helpers.Types

{
	public readonly struct Neighbor
	{
		#region Declared

		public const float DIAGONAL_DISTANCE = 1.41421f;
		public const float SIMPLE_DISTANCE = 1f;

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
			this.Distance = x != 0 && y != 0 ? DIAGONAL_DISTANCE : SIMPLE_DISTANCE;
			// In struct first need to assign fields than can call methods
			CheckValues( x, y );
		}

		public static NativeArray<Neighbor> FullNeighborhood( Allocator allocator )
		{
			return new NativeArray<Neighbor>( 8, allocator )
			{
				[0] = Neighbor.UPPER_LEFT,
				[1] = Neighbor.UPPER,
				[2] = Neighbor.UPPER_RIGHT,
				[3] = Neighbor.LEFT,
				[4] = Neighbor.RIGHT,
				[5] = Neighbor.BOTTOM_LEFT,
				[6] = Neighbor.BOTTOM,
				[7] = Neighbor.BOTTOM_RIGHT,
			};
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