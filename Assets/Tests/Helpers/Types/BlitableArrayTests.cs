using Helpers.Types;

using NUnit.Framework;

using System;
using System.Linq;

using Tests.Categories;

using Unity.Collections;

using UnityEngine.TestTools;

using static NUnit.Framework.Assert;

namespace Tests.Helpers.Types
{
	public class BlitableArrayTests
	{
		[Test]
		[UtilsTest]
		public void Size()
		{
			var array = new BlitableArray<float>();
			Zero( array.Length );
			array.Dispose();

			array = new BlitableArray<float>( 10, Allocator.Temp );
			AreEqual( 10, array.Length );
			array.Dispose();
		}

		[Test]
		[UtilsTest]
		public void To_ManagedArray_Implicit()
		{
			var managedArray = Enumerable.Range(0, 25).Select(x => (float)x).ToArray();
			var array = new BlitableArray<float>();

			array.Allocate( managedArray, Allocator.Temp );
			managedArray = array;

			AreEqual( array.Length, managedArray.Length );

			for ( int i = 0; i < array.Length; i++ )
			{
				AreEqual( array[i], managedArray[i] );
			}

			array.Dispose();
		}

		[Test]
		[UtilsTest]
		public void To_ManagedArray_Call()
		{
			var managedArray = Enumerable.Range(0, 25).Select(x => (float)x).ToArray();
			var array = new BlitableArray<float>();

			array.Allocate( managedArray, Allocator.Temp );
			managedArray = array.ToArray();

			AreEqual( array.Length, managedArray.Length );

			for ( int i = 0; i < array.Length; i++ )
			{
				AreEqual( array[i], managedArray[i] );
			}

			array.Dispose();
		}

		[Test]
		[UtilsTest]
		public void To_NativeArray_Implicit()
		{
			var managedArray = Enumerable.Range(0, 25).Select(x => (float)x).ToArray();
			var array = new BlitableArray<float>();

			array.Allocate( managedArray, Allocator.Temp );
			NativeArray<float> nativeArray = array;

			AreEqual( array.Length, nativeArray.Length );

			for ( int i = 0; i < array.Length; i++ )
			{
				AreEqual( array[i], nativeArray[i] );
			}

			nativeArray.Dispose();
			array.Dispose();
		}

		[Test]
		[UtilsTest]
		public void To_NativeArray_Call()
		{
			var managedArray = Enumerable.Range(0, 25).Select(x => (float)x).ToArray();
			var array = new BlitableArray<float>();

			array.Allocate( managedArray, Allocator.Temp );
			NativeArray<float> nativeArray = array.ToNativeArray(Allocator.Temp);

			AreEqual( array.Length, nativeArray.Length );

			for ( int i = 0; i < array.Length; i++ )
			{
				AreEqual( array[i], nativeArray[i] );
			}

			nativeArray.Dispose();
			array.Dispose();
		}

		[Test]
		[UtilsTest]
		public void To_NativeArray_Zero()
		{
			var array = new BlitableArray<float>();

			NativeArray<float> nativeArray = array.ToNativeArray(Allocator.Temp);

			AreEqual( array.Length, nativeArray.Length );

			for ( int i = 0; i < array.Length; i++ )
			{
				AreEqual( array[i], nativeArray[i] );
			}

			nativeArray.Dispose();
			array.Dispose();
		}

		public class Constructor
		{
			[Test]
			[UtilsTest]
			public void Constructor_float_Empty()
			{
				DoesNotThrow( () =>
				 {
					 var array = new BlitableArray<float>();
					 array.Dispose();
				 } );
			}

			[Test]
			[UtilsTest]
			public void Constructor_float_Zero()
			{
				DoesNotThrow( () =>
				{
					var array = new BlitableArray<float>(0, Allocator.Temp);
					array.Dispose();
				} );
			}

			[Test]
			[UtilsTest]
			public void Constructor_float_Minus()
			{
				Throws<ArgumentException>( () =>
				{
					var array = new BlitableArray<float>(-1, Allocator.Temp);
					array.Dispose();
				} );
			}

			[Test]
			[UtilsTest]
			public void Constructor_bool()
			{
				DoesNotThrow( () =>
				{
					var array = new BlitableArray<bool>();
					array.Dispose();
				} );

				Throws<ArgumentException>( () =>
				{
					var array = new BlitableArray<bool>(0, Allocator.Temp);
					array.Dispose();
				} );
			}

			[Test]
			[UtilsTest]
			public void Constructor_float_Temp()
			{
				DoesNotThrow( () =>
				{
					var array = new BlitableArray<float>(10, Allocator.Temp);
					array.Dispose();
				} );
			}

			[Test]
			[UtilsTest]
			public void Constructor_float_Persistent()
			{
				DoesNotThrow( () =>
				{
					var array = new BlitableArray<float>(10, Allocator.Persistent);
					array.Dispose();
				} );
			}

			[Test]
			[UtilsTest]
			public void Constructor_float_TempJob()
			{
				DoesNotThrow( () =>
				{
					var array = new BlitableArray<float>(10, Allocator.TempJob);
					array.Dispose();
				} );
			}

			[Test]
			[UtilsTest]
			public void Constructor_float_Invalid()
			{
				LogAssert.ignoreFailingMessages = true;
				Throws<ArgumentException>( () =>
				{
					var array = new BlitableArray<float>(10, Allocator.Invalid);
					array.Dispose();
				} );
			}

			[Test]
			[UtilsTest]
			public void Constructor_float_None()
			{
				LogAssert.ignoreFailingMessages = true;
				Throws<ArgumentException>( () =>
				{
					var array = new BlitableArray<float>(10, Allocator.None);
					array.Dispose();
				} );
			}
		}

		public class Alocation
		{
			[Test]
			[UtilsTest]
			public void Alocation_Count_Minus()
			{
				Throws<ArgumentException>( () =>
				{
					var array = new BlitableArray<float>();
					array.Allocate( -1, Allocator.Temp );
					array.Dispose();
				} );
			}

			[Test]
			[UtilsTest]
			public void Alocation_Count_Zero()
			{
				DoesNotThrow( () =>
				{
					var array = new BlitableArray<float>();
					array.Allocate( 0, Allocator.Temp );
					array.Dispose();
				} );
			}

			[Test]
			[UtilsTest]
			public void Alocation_Count_Ten_Temp()
			{
				DoesNotThrow( () =>
				{
					var array = new BlitableArray<float>();
					array.Allocate( 10, Allocator.Temp );
					array.Dispose();
				} );
			}

			[Test]
			[UtilsTest]
			public void Alocation_Count_Ten_TempJob()
			{
				DoesNotThrow( () =>
				{
					var array = new BlitableArray<float>();
					array.Allocate( 10, Allocator.TempJob );
					array.Dispose();
				} );
			}

			[Test]
			[UtilsTest]
			public void Alocation_Count_Ten_Persistent()
			{
				DoesNotThrow( () =>
				{
					var array = new BlitableArray<float>();
					array.Allocate( 10, Allocator.Persistent );
					array.Dispose();
				} );
			}

			[Test]
			[UtilsTest]
			public void Alocation_Count_Ten_Invalid()
			{
				LogAssert.ignoreFailingMessages = true;
				Throws<ArgumentException>( () =>
				{
					var array = new BlitableArray<float>();
					array.Allocate( 10, Allocator.Invalid );
					array.Dispose();
				} );
			}

			[Test]
			[UtilsTest]
			public void Alocation_Count_Ten_None()
			{
				LogAssert.ignoreFailingMessages = true;
				Throws<ArgumentException>( () =>
				{
					var array = new BlitableArray<float>();
					array.Allocate( 10, Allocator.None );
					array.Dispose();
				} );
			}

			[Test]
			[UtilsTest]
			public void Alocation_From_ManagedArray()
			{
				var managedArray = Enumerable.Range(0, 25).Select(x => (float)x).ToArray();
				var array = new BlitableArray<float>();

				array.Allocate( managedArray, Allocator.Temp );

				AreEqual( managedArray.Length, array.Length );

				for ( int i = 0; i < array.Length; i++ )
				{
					AreEqual( managedArray[i], array[i] );
				}

				array.Dispose();
			}

			[Test]
			[UtilsTest]
			public void Alocation_From_List()
			{
				var list = Enumerable.Range(0, 25).Select(x => (float)x).ToList();
				var array = new BlitableArray<float>();

				array.Allocate( list, Allocator.Temp );

				AreEqual( list.Count, array.Length );

				for ( int i = 0; i < array.Length; i++ )
				{
					AreEqual( list[i], array[i] );
				}

				array.Dispose();
			}

			[Test]
			[UtilsTest]
			public void Alocation_From_Other()
			{
				var managedArray = Enumerable.Range(0, 25).Select(x => (float)x).ToArray();
				var otherArray = new NativeArray<float>(managedArray, Allocator.Temp);
				var array = new BlitableArray<float>();

				array.Allocate( otherArray, Allocator.Temp );

				AreEqual( otherArray.Length, array.Length );

				for ( int i = 0; i < array.Length; i++ )
				{
					AreEqual( otherArray[i], array[i] );
				}

				otherArray.Dispose();
				array.Dispose();
			}
		}
	}
}