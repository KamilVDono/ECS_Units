using Helpers;

using Pathfinding.Helpers;

using Unity.Entities;

namespace Pathfinding.Components
{
	public struct MapSettingsNeighborsState : IComponentData
	{
		public BlitableArray<Neighbor> Neighbours;
	}
}