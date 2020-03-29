using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Rendering.Components
{
	[MaterialProperty( "_RowsColumns_Tex_Anim", MaterialPropertyFormat.Float4 )]
	public struct RowsColumns_Tex_AnimMaterialProperty : IComponentData
	{
		public float4 Value;
	}
}