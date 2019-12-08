using Unity.Entities;

namespace Pathfinding.Components
{
	public struct MovementCost : IComponentData
	{
		public const float FREE = 0;
		public const float IMPOSSIBLE = float.MaxValue;

		public float Cost;
	}
}