using System;

using Unity.Mathematics;

namespace Helpers.Types
{
	[Serializable]
	public struct IntRange
	{
		public int Min;
		public int Max;

		public int RandomPick => RandomUtils.RandomValue( Min, Max );

		public IntRange( int min, int max )
		{
			Min = math.min( min, max );
			Max = math.max( min, max );
		}

		public static explicit operator int( IntRange range ) => range.RandomPick;
	}
}