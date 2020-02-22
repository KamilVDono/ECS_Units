using Unity.Mathematics;

namespace Helpers.Types
{
	[System.Serializable]
	public struct FloatRange
	{
		public float Min;
		public float Max;

		public float RandomPick => RandomUtils.RandomValue( Min, Max );

		public FloatRange( float min, float max )
		{
			Min = math.min( min, max );
			Max = math.max( min, max );
		}

		public static explicit operator float( FloatRange range ) => range.RandomPick;
	}
}