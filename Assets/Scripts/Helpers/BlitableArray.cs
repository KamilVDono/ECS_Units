using System;

using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Helpers
{
	[NativeContainer]
	public unsafe struct BlitableArray<T> : IDisposable where T : struct
	{
		private Allocator _allocator;

		[NativeDisableUnsafePtrRestriction]
		private void* _buffer;

		public int Length { get; private set; }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
		private AtomicSafetyHandle m_Safety;
#endif

		public BlitableArray( int length, Allocator allocator ) : this()
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
			if ( !UnsafeUtility.IsBlittable<T>() )
			{
				throw new ArgumentException( string.Format( "{0} used in NativeCustomArray<{0}> must be blittable", typeof( T ) ) );
			}
#endif

			Length = length;
			_allocator = allocator;
			Allocate( length, allocator );
		}

		public unsafe T this[int index]
		{
			get
			{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
				if ( _allocator == Allocator.Invalid )
				{
					throw new ArgumentException( "AutoGrowArray was not initialized." );
				}

				if ( index >= Length )
				{
					throw new IndexOutOfRangeException();
				}
#endif

				return UnsafeUtility.ReadArrayElement<T>( _buffer, index );
			}

			set
			{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
				if ( _allocator == Allocator.Invalid )
				{
					throw new ArgumentException( "AutoGrowArray was not initialized." );
				}

				if ( index >= Length )
				{
					throw new IndexOutOfRangeException();
				}
#endif
				UnsafeUtility.WriteArrayElement( _buffer, index, value );
			}
		}

		public static implicit operator NativeArray<T>( BlitableArray<T> array )
		{
			NativeArray<T> na = new NativeArray<T>(array.Length, array._allocator);
			for ( var i = 0; i < array.Length; i++ )
			{
				na[i] = array[i];
			}
			return na;
		}

		public static implicit operator T[]( BlitableArray<T> array )
		{
			return array.ToArray();
		}

		public unsafe void Allocate( T[] array, Allocator allocator )
		{
			Allocate( array.Length, allocator );

			for ( var i = 0; i < Length; i++ )
			{
				this[i] = array[i];
			}
		}

		public unsafe void Allocate( NativeArray<T> array, Allocator allocator )
		{
			Allocate( array.Length, allocator );

			for ( var i = 0; i < Length; i++ )
			{
				this[i] = array[i];
			}
		}

		public unsafe void Allocate( int size, Allocator allocator )
		{
			_allocator = allocator;
			Length = size;
			var elementSize = UnsafeUtility.SizeOf<T>();
			_buffer = UnsafeUtility.Malloc( size * elementSize, UnsafeUtility.AlignOf<T>(), allocator );
		}

		public unsafe void Dispose()
		{
			UnsafeUtility.Free( _buffer, _allocator );
		}

		public T[] ToArray()
		{
			var res = new T[Length];

			for ( var i = 0; i < Length; i++ )
			{
				res[i] = this[i];
			}

			return res;
		}
	}
}