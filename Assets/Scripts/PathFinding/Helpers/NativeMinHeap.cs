using System;

using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace PathFinding.Helpers
{
	[NativeContainerSupportsDeallocateOnJobCompletion]
	[NativeContainer]
	public unsafe struct NativeMinHeap : IDisposable
	{
		private readonly Allocator _allocator;

		[NativeDisableUnsafePtrRestriction]
		private void* _buffer;

		private int _capacity;

		private int _head;

		private int _length;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
		private AtomicSafetyHandle m_Safety;

		[NativeSetClassTypeToNullOnSchedule]
		private DisposeSentinel m_DisposeSentinel;

#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="NativeMinHeap"/> struct.
		/// </summary>
		/// <param name="capacity">The capacity of the min heap.</param>
		/// <param name="allocator">The allocator.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if allocator not set, capacity is negative or the size &gt; maximum integer value.
		/// </exception>
		public NativeMinHeap( int capacity, Allocator allocator )
		{
			var size = (long)UnsafeUtility.SizeOf<MinHeapNode>() * capacity;
			if ( allocator <= Allocator.None )
			{
				throw new ArgumentException( "Allocator must be Temp, TempJob or Persistent", nameof( allocator ) );
			}

			if ( capacity < 0 )
			{
				throw new ArgumentOutOfRangeException( nameof( capacity ), "Length must be >= 0" );
			}

			if ( size > int.MaxValue )
			{
				throw new ArgumentOutOfRangeException(
					nameof( capacity ),
					$"Length * sizeof(T) cannot exceed {int.MaxValue} bytes" );
			}

			this._buffer = UnsafeUtility.Malloc( size, UnsafeUtility.AlignOf<MinHeapNode>(), allocator );
			this._capacity = capacity;
			this._allocator = allocator;
			this._head = -1;
			this._length = 0;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
			DisposeSentinel.Create( out this.m_Safety, out this.m_DisposeSentinel, 1, allocator );
#endif
		}

		/// <summary>
		/// Does the heap still have remaining nodes.
		/// </summary>
		/// <returns>True if the min heap still has at least one remaining node, otherwise false.</returns>
		public bool HasNext()
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
			AtomicSafetyHandle.CheckReadAndThrow( this.m_Safety );
#endif
			return this._head >= 0;
		}

		/// <summary>
		/// Add a node to the heap which will be sorted.
		/// </summary>
		/// <param name="node">The node to add.</param>
		/// <exception cref="IndexOutOfRangeException">Throws if capacity reached.</exception>
		public void Push( MinHeapNode node )
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
			if ( this._length == this._capacity )
			{
				throw new IndexOutOfRangeException( "Capacity Reached" );
			}

			AtomicSafetyHandle.CheckReadAndThrow( this.m_Safety );
#endif
			if ( this._head < 0 )
			{
				this._head = this._length;
			}
			else if ( node.ExpectedCost < this.Get( this._head ).ExpectedCost )
			{
				node.Next = this._head;
				this._head = this._length;
			}
			else
			{
				var currentPtr = this._head;
				var current = this.Get(currentPtr);

				while ( current.Next >= 0 && this.Get( current.Next ).ExpectedCost <= node.ExpectedCost )
				{
					currentPtr = current.Next;
					current = this.Get( current.Next );
				}

				node.Next = current.Next;
				current.Next = this._length;

				UnsafeUtility.WriteArrayElement( this._buffer, currentPtr, current );
			}

			UnsafeUtility.WriteArrayElement( this._buffer, this._length, node );
			this._length += 1;
		}

		/// <summary>
		/// Take the top node off the heap.
		/// </summary>
		/// <returns>The current node of the heap.</returns>
		public MinHeapNode Pop()
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
			AtomicSafetyHandle.CheckWriteAndThrow( this.m_Safety );
#endif
			var result = this._head;
			this._head = this.Get( this._head ).Next;
			return this.Get( result );
		}

		/// <summary>
		/// Clear the heap by resetting the head and length.
		/// </summary>
		/// <remarks>Does not clear memory.</remarks>
		public void Clear()
		{
			this._head = -1;
			this._length = 0;
		}

		/// <summary>
		/// Dispose of the heap by freeing up memory.
		/// </summary>
		/// <exception cref="InvalidOperationException">Memory hasn't been allocated.</exception>
		public void Dispose()
		{
			if ( !UnsafeUtility.IsValidAllocator( this._allocator ) )
			{
				return;
			}
#if ENABLE_UNITY_COLLECTIONS_CHECKS
			DisposeSentinel.Dispose( ref this.m_Safety, ref this.m_DisposeSentinel );
#endif
			UnsafeUtility.Free( this._buffer, this._allocator );
			this._buffer = null;
			this._capacity = 0;
		}

		public NativeMinHeap Slice( int start, int length )
		{
			var stride = UnsafeUtility.SizeOf<MinHeapNode>();

			return new NativeMinHeap()
			{
				_buffer = (byte*)( (IntPtr)this._buffer + stride * start ),
				_capacity = length,
				_length = 0,
				_head = -1,
#if ENABLE_UNITY_COLLECTIONS_CHECKS
				m_Safety = this.m_Safety,
#endif
			};
		}

		private MinHeapNode Get( int index )
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
			if ( index < 0 || index >= this._length )
			{
				this.FailOutOfRangeError( index );
			}

			AtomicSafetyHandle.CheckReadAndThrow( this.m_Safety );
#endif

			return UnsafeUtility.ReadArrayElement<MinHeapNode>( this._buffer, index );
		}

#if ENABLE_UNITY_COLLECTIONS_CHECKS

		private void FailOutOfRangeError( int index ) => throw new IndexOutOfRangeException( $"Index {index} is out of range of '{this._capacity}' Length." );

#endif
	}

	/// <summary>
	/// The min heap node.
	/// </summary>
	public struct MinHeapNode
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MinHeapNode"/> struct.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="expectedCost">The expected cost.</param>
		/// <param name="distanceToGoal">Remaining distance to the goal</param>
		public MinHeapNode( int position, float expectedCost )
		{
			this.Position = position;
			this.ExpectedCost = expectedCost;
			this.Next = -1;
		}

		/// <summary>
		/// Gets the position.
		/// </summary>
		public int Position { get; }

		/// <summary>
		/// Gets the expected cost.
		/// </summary>
		public float ExpectedCost { get; }

		/// <summary>
		/// Gets or sets the next node in the heap.
		/// </summary>
		public int Next { get; set; }
	}
}