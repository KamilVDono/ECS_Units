using Unity.Entities;
using Unity.Rendering;

namespace Rendering.Components
{
	[MaterialProperty( "_AnimationSpeed", MaterialPropertyFormat.Float )]
	public struct AnimationSpeedMaterialProperty : IComponentData
	{
		public float Speed;
	}
}