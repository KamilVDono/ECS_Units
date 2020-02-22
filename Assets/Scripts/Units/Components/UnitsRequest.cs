using Helpers.Types;

using Unity.Entities;
using Unity.Mathematics;

namespace Units.Components
{
	public struct UnitsRequest : IComponentData
	{
		public int UnitsCount;
		public FloatRange UnitSpeed;
		public FloatRange UnitMiningSpeed;

		// texture data
		public int2 TextureTiles;
	}
}