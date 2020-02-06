using System;

using Unity.Entities;
using Unity.Mathematics;

namespace Maps.Components
{
	public struct MapIndex : IComponentData, IEquatable<MapIndex>
	{
		public int Index1D;
		public int2 Index2D;

		public MapIndex( int index1D, int2 index2D )
		{
			Index1D = index1D;
			Index2D = index2D;
		}

		public MapIndex( in MapIndex other )
		{
			Index1D = other.Index1D;
			Index2D = other.Index2D;
		}

		public static MapIndex From( int index1D, int edgeSize )
		{
			var index2D = Helpers.IndexUtils.Index2DEdge( index1D, edgeSize );
			return new MapIndex( index1D, index2D );
		}

		public static MapIndex From( int2 index2D, int edgeSize )
		{
			var index1D = Helpers.IndexUtils.Index1DEdge( index2D, edgeSize );
			return new MapIndex( index1D, index2D );
		}

		public static MapIndex FromWorldPosition( float2 position, int edgeSize )
		{
			var index2D = Helpers.IndexUtils.WorldIndex2D(position);
			return MapIndex.From( index2D, edgeSize );
		}

		public static MapIndex FromWorldPosition( float3 position, int edgeSize )
		{
			var index2D = Helpers.IndexUtils.WorldIndex2D(new float2(position.x, position.z));
			return MapIndex.From( index2D, edgeSize );
		}

		public override string ToString() => $"Index2 {Index2D}. Index1 {Index1D}";

		public bool Equals( MapIndex other ) => Index1D == other.Index1D;
	}
}