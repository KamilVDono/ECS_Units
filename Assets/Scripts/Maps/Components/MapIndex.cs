using Unity.Entities;
using Unity.Mathematics;

namespace Maps.Components
{
	public struct MapIndex : IComponentData
	{
		private int _index1D;
		private int2 _index2D;
		public int Index1D => _index1D;
		public int2 Index2D => _index2D;

		public MapIndex( int index1D, int2 index2D )
		{
			_index1D = index1D;
			_index2D = index2D;
		}

		public static MapIndex From( int index1D, int edgeSize )
		{
			var index2D = Helpers.IndexUtils.Index2D( index1D, edgeSize );
			return new MapIndex( index1D, index2D );
		}

		public static MapIndex From( int2 index2D, int edgeSize )
		{
			var index1D = Helpers.IndexUtils.Index1D( index2D, edgeSize );
			return new MapIndex( index1D, index2D );
		}
	}
}