using Unity.Entities;

namespace Resources.Components
{
	/// <summary>
	/// Resource ore change mean that the ore has been depleted
	/// </summary>
	public struct ResourceOreChange : IComponentData
	{
		public Entity oreEntity;
	}
}