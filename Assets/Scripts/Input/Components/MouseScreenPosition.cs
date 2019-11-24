using Unity.Entities;
using Unity.Mathematics;

namespace Input.Components
{
	public struct MouseScreenPosition : IComponentData
	{
		public int2 Position;
		public int2 Delta;
	}
}
