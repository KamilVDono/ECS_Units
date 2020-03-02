using Unity.Entities;


namespace Input.Components
{
	public struct CurrentMouseMode : IComponentData
	{
		public MouseMode Mode;
	}

	public enum MouseMode
	{
		Mining,
		Building,
		PathFinding,
		None = -1,
	}
}