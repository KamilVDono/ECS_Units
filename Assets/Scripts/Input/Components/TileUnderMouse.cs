using Unity.Entities;

namespace Input.Components
{
	public struct TileUnderMouse : IComponentData
	{
		public Entity Tile;
	}
}
