using Unity.Entities;
using Unity.Mathematics;

namespace Input.Components
{
	public struct MouseWorldPosition : IComponentData
	{
		public float2 Position;
		public float2 Delta;
	}
}
