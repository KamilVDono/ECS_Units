using NUnit.Framework;

using System.Linq;

using Tests.Categories;

using Unity.Collections;

using static Helpers.RandomUtils;
using static NUnit.Framework.Assert;

namespace Tests.Helpers
{
	public class RandomUtilsTests
	{
		[Test]
		[UtilsTest]
		public void RandomValue_Int_0_1()
		{
			var randomValues = Enumerable.Range(1, 100).Select(_ => RandomValue( 0, 1 )).ToList();
			True( randomValues.All( v => v < 1 ) );
			True( randomValues.All( v => v >= 0 ) );
		}

		[Test]
		[UtilsTest]
		public void RandomValue_Int_0_10()
		{
			var randomValues = Enumerable.Range(1, 500).Select(_ => RandomValue( 0, 10 )).ToList();
			True( randomValues.All( v => v < 10 ) );
			True( randomValues.All( v => v >= 0 ) );
			True( randomValues.Any( v => v >= 9 ) );
		}

		[Test]
		[UtilsTest]
		public void RandomValue_Int_Wrong_Order() => DoesNotThrow( () => RandomValue( 10, 0 ) );

		[Test]
		[UtilsTest]
		public void RandomValue_Int_No_span()
		{
			var randomValues = Enumerable.Range(1, 100).Select(_ => RandomValue( 0, 0 )).ToList();
			True( randomValues.All( v => v == 0 ) );
		}

		[Test]
		[UtilsTest]
		public void RandomValue_Float_0_1()
		{
			var randomValues = Enumerable.Range(1, 500).Select(_ => RandomValue( 0f, 1f )).ToList();
			True( randomValues.All( v => v < 1 ) );
			True( randomValues.All( v => v >= 0 ) );
		}

		[Test]
		[UtilsTest]
		public void RandomValue_Float_0_10()
		{
			var randomValues = Enumerable.Range(1, 500).Select(_ => RandomValue( 0f, 10f )).ToList();
			True( randomValues.All( v => v < 10 ) );
			True( randomValues.All( v => v >= 0 ) );
			True( randomValues.Any( v => v >= 9 ) );
		}

		[Test]
		[UtilsTest]
		public void RandomValue_Float_Wrong_Order() => DoesNotThrow( () => RandomValue( 10f, 0f ) );

		[Test]
		[UtilsTest]
		public void RandomValue_Float_No_span()
		{
			var randomValues = Enumerable.Range(1, 100).Select(_ => RandomValue( 0f, 0f )).ToList();
			True( randomValues.All( v => v == 0f ) );
		}

		[Test]
		[UtilsTest]
		public void RandomPick_Collection_0_10()
		{
			var randomValues = Enumerable.Range(1, 50).Select(_ => RandomValue( 0f, 10f )).ToList();
			for ( int i = 0; i < randomValues.Count * 100; i++ )
			{
				DoesNotThrow( () => randomValues.RandomPick() );
			}
		}

		[Test]
		[UtilsTest]
		public void RandomPick_Array_0_10()
		{
			var randomValues = Enumerable.Range(1, 50).Select(_ => RandomValue( 0f, 10f )).ToArray();
			for ( int i = 0; i < randomValues.Length * 100; i++ )
			{
				DoesNotThrow( () => randomValues.RandomPick() );
			}
		}

		[Test]
		[UtilsTest]
		public void RandomPick_NativeArray_0_10()
		{
			var randomValues = Enumerable.Range(1, 50).Select(_ => RandomValue( 0f, 10f )).ToArray();
			var nativeRandomArray = new NativeArray<float>(randomValues.Length, Allocator.Temp);
			nativeRandomArray.CopyFrom( randomValues );

			for ( int i = 0; i < nativeRandomArray.Length * 100; i++ )
			{
				DoesNotThrow( () => nativeRandomArray.RandomPick() );
			}

			nativeRandomArray.Dispose();
		}
	}
}