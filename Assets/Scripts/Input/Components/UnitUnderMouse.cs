using Unity.Entities;

namespace Input.Components
{
	public struct UnitUnderMouse : IComponentData
	{
		public Entity Unit;
	}
}