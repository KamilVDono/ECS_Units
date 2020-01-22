using System;

using Unity.Entities;

namespace Input.Components
{
	[Flags]
	public enum MouseButton : byte
	{
		None = 0,
		Left   = 1 << 0,
		Right  = 1 << 1,
		Middle = 1 << 2,
	}

	public struct MouseButtons : IComponentData
	{
		public MouseButton Previous;
		public MouseButton Current;
	}
}