using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Helpers.Types
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
				throw new ArgumentException( string.Format( "{0} used in BlitableArray<{0}> must be blittable", typeof( T ) ) );
			}
#endif
			Allocate( length, allocator );
		}

		public unsafe T this[int index]
		{
			get
			{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
				if ( _allocator == Allocator.Invalid )
				{
					throw new ArgumentException( "BlitableArray was not initialized." );
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

		public static implicit operator NativeArray<T>( BlitableArray<T> array ) => array.ToNativeArray( array._allocator );

		public static implicit operator T[]( BlitableArray<T> array ) => array.ToArray();

		public unsafe void Allocate( ICollection<T> array, Allocator allocator )
		{
			Allocate( array.Count, allocator );

			for ( var i = 0; i < Length; i++ )
			{
				this[i] = array.ElementAt( i );
			}
		}

		public unsafe void Allocate( NativeArray<T> array, Allocator allocator )
		{
			Allocate( array.Length, allocator );
			UnsafeUtility.MemCpy( _buffer, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks( array ), Length * UnsafeUtility.SizeOf<T>() );
		}

		public unsafe void Allocate( int size, Allocator allocator )
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
			if ( size < 0 )
			{
				throw new ArgumentException( "Size must be at least 0" );
			}
#endif
			Dispose();
			_allocator = allocator;
			Length = size;
			var elementSize = UnsafeUtility.SizeOf<T>();
			_buffer = UnsafeUtility.Malloc( size * elementSize, UnsafeUtility.AlignOf<T>(), allocator );
		}

		public unsafe void Dispose()
		{
			if ( _allocator == Allocator.Invalid || _allocator == Allocator.None )
			{
				return;
			}
			UnsafeUtility.Free( _buffer, _allocator );
			_allocator = Allocator.Invalid;
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

		public NativeArray<T> ToNativeArray( Allocator allocator )
		{
			NativeArray<T> na = new NativeArray<T>(Length, allocator);
			UnsafeUtility.MemCpy( NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks( na ), _buffer, Length * UnsafeUtility.SizeOf<T>() );
			return na;
		}
	}
}