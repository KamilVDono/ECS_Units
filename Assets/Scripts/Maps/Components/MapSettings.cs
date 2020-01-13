using Helpers;

using Unity.Entities;

namespace Maps.Components
{
	public struct MapSettings : ISharedComponentData
	{
		public int MapEdgeSize;
		public BlitableArray<Entity> Tiles;
		public BlitableArray<Entity> Resources;
	}
}