using Unity.Entities;
using Unity.Mathematics;

namespace PathFinding.Components
{
	public struct Waypoint : IBufferElementData
	{
		public float2 Position;
	}
}