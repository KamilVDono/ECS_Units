using Unity.Entities;
using Unity.Mathematics;

namespace Units.Components
{
	public struct UnitsRequest : IComponentData
	{
		public int UnitsCount;
		public float UnitSpeed;

		// texture data
		public int2 TextureTiles;
	}
}