using Helpers;

using Unity.Entities;

namespace Maps.Components
{
	public struct MapSettings : IComponentData
	{
		public int MapEdgeSize;
		public BlitableArray<Entity> Tiles;
	}
}