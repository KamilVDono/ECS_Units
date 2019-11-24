using Unity.Entities;

namespace PathFinding.Components
{
	public struct MovementCost : IComponentData
	{
		public const float FREE = 0;
		public const float IMPOSIBLE = float.MaxValue;

		public float Cost;
	}
}