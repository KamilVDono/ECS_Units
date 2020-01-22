using Helpers;

using Input.Components;

using Maps.Components;

using Unity.Entities;

namespace Input.Systems
{
	public class MouseOverTileSystem : ComponentSystem
	{
		protected override void OnCreate()
		{
			RequireSingletonForUpdate<MapSettings>();
			EntityManager.CreateEntity( typeof( TileUnderMouse ), typeof( MouseWorldPosition ) );
		}

		protected override void OnUpdate()
		{
			var mapSettings = GetSingleton<MapSettings>();

			Entities
				.ForEach( ( ref TileUnderMouse tileUnderMouse, ref MouseWorldPosition mouseWorldPosition ) =>
				{
					if ( mapSettings.Tiles.Length != 0 )
					{
						tileUnderMouse.Tile = mapSettings.Tiles[IndexUtils.WorldIndex1D( mouseWorldPosition.Position, mapSettings.Tiles.Length )];
					}
					else
					{
						tileUnderMouse.Tile = new Entity();
					}
				} );
		}
	}
}