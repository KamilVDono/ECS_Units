using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Rendering.Components
{
	[MaterialProperty( "_MainColor", MaterialPropertyFormat.Float4 )]
	public struct MainColorMaterialProperty : IComponentData
	{
		public float4 Color;
	}
}