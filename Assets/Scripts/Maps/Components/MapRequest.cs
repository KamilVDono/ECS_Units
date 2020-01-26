using Unity.Entities;
using Unity.Mathematics;

namespace Maps.Components
{
	public struct MapRequest : IComponentData
	{
		public float2 Frequency;
		public float2 Offset;
		public int MapEdgeSize;
	}
}