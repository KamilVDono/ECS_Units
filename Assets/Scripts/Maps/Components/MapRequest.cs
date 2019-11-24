using Helpers;

using Unity.Entities;
using Unity.Mathematics;

namespace Maps.Components
{
	public struct MapRequest : ISharedComponentData
	{
		public float2 Frequency;
		public BlitableArray<TileType> TileTypes;
	}
}