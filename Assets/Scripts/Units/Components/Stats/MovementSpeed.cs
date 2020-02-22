using Unity.Entities;

namespace Units.Components.Stats
{
	public struct MovementSpeed : IComponentData, IUnitStat
	{
		public float Speed;
	}
}