using Helpers;

using Input.Components;

using Maps.Components;

using Unity.Entities;

namespace Scripts.Assets.Scripts.Input.Systems
{
	public class MouseOverTileSystem : ComponentSystem
	{
		protected override void OnCreate() => EntityManager.CreateEntity( typeof( TileUnderMouse ), typeof( MouseWorldPosition ) );

		protected override void OnUpdate() => Entities.ForEach( ( ref TileUnderMouse tileUnderMouse, ref MouseWorldPosition mouseWorldPosition ) =>
		{
			BlitableArray<Entity> tiles = new BlitableArray<Entity>();

			Entities.ForEach( ( ref MapSettings mapSettings ) =>
			{
				tiles = mapSettings.Tiles;
			} );

			if ( tiles.Length != 0 )
			{
				tileUnderMouse.Tile = tiles[IndexUtils.WorldIndex1D( mouseWorldPosition.Position, tiles.Length )];
			}
			else
			{
				tileUnderMouse.Tile = new Entity();
			}
		} );
	}
}