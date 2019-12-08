using Unity.Entities;
using Unity.Mathematics;

namespace Pathfinding.Components
{
	public struct Waypoint : IBufferElementData
	{
		public int2 Position;
	}
}