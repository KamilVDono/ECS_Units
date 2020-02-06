using Unity.Entities;

namespace Units.Components
{
	public struct UnitsRequest : IComponentData
	{
		public int UnitsCount;
		public float UnitSpeed;
	}
}