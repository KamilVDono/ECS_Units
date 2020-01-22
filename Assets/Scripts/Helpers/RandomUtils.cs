using System.Collections.Generic;

using Unity.Collections;
using Unity.Mathematics;

namespace Helpers
{
	public static class RandomUtils
	{
		private static Random _random = new Random();

		static RandomUtils()
		{
			_random.InitState();
		}

		public static T RandomPick<T>( this List<T> list )
		{
			return list[_random.NextInt( list.Count )];
		}

		public static T RandomPick<T>( this T[] array )
		{
			return array[_random.NextInt( array.Length )];
		}

		public static T RandomPick<T>( this NativeArray<T> array ) where T : struct
		{
			return array[_random.NextInt( array.Length )];
		}
	}
}