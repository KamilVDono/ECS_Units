using System.Collections.Generic;
using System.Linq;

using Unity.Collections;
using Unity.Mathematics;

namespace Helpers
{
	public static class RandomUtils
	{
		private static Random _random = new Random();

		static RandomUtils() => _random.InitState();

		/// <summary>
		/// Returns a uniformly random int value in the interval [min, max).
		/// </summary>
		/// <param name="min">Inclusive</param>
		/// <param name="max">Exclusive</param>
		public static int RandomValue( int min, int max ) => _random.NextInt( min, max );

		/// <summary>
		/// Returns a uniformly random float value in the interval [min, max).
		/// </summary>
		/// <param name="min">Inclusive</param>
		/// <param name="max">Exclusive</param>
		public static float RandomValue( float min, float max ) => _random.NextFloat( min, max );

		public static T RandomPick<T>( this ICollection<T> list ) => list.ElementAt( _random.NextInt( list.Count ) );

		public static T RandomPick<T>( this NativeArray<T> array ) where T : struct => array[_random.NextInt( array.Length )];
	}
}