using Helpers;

using PathFinding.Helpers;

using Unity.Entities;

namespace PathFinding.Components
{
	internal struct MapSettingsNeighborsState : ISystemStateComponentData
	{
		public BlitableArray<Neighbor> Neighbours;
	}
}