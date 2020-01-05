using Unity.Entities;

namespace Pathfinding.Components
{
	public struct MovementCost : IComponentData
	{
		public const float FREE = 0;
		public const float IMPOSSIBLE = float.MaxValue;

		public float Cost;

		public override string ToString()
		{
			if ( Cost == IMPOSSIBLE )
			{
				return "Movement cost IMPOSSIBLE";
			}
			else if ( Cost == FREE )
			{
				return "Movement cost FREE";
			}
			return $"Movement cost {Cost};";
		}
	}
}