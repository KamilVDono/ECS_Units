using Helpers.Types;

using Unity.Entities;

namespace Maps.Components
{
	// TODO: Change to blob as it better for that kind of data
	public struct MapSettings : IComponentData
	{
		public int MapEdgeSize;
		public BlitableArray<Entity> Tiles;
	}
}