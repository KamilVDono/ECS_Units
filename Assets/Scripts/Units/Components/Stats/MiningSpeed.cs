using Unity.Entities;

namespace Units.Components.Stats
{
	public struct MiningSpeed : IComponentData, IUnitStat
	{
		public float Speed;
	}
}