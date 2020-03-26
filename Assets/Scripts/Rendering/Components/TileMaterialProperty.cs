using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Rendering.Components
{
	[MaterialProperty( "_Tile", MaterialPropertyFormat.Float4 )]
	public struct TileMaterialProperty : IComponentData
	{
		public float4 Tile;
	}
}