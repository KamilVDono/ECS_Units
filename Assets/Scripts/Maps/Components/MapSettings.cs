using Helpers;

using Unity.Entities;

namespace Maps.Components
{
	public struct MapSettings : IComponentData
	{
		public Boolean CanMoveDiagonally;
		public int MapSize;
		public BlitableArray<Entity> Tiles;
	}
}