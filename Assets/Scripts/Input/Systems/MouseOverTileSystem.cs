using Helpers;

using Input.Components;

using Maps.Components;
using Maps.Systems;

using Unity.Entities;

namespace Input.Systems
{
	public class MouseOverTileSystem : ComponentSystem, IRequiresMapSettings
	{
		public Entity MapSettingsEntity { get; set; }
		private MapSettings MapSettings { get; set; }

		protected override void OnCreate() => EntityManager.CreateEntity( typeof( TileUnderMouse ), typeof( MouseWorldPosition ) );

		protected override void OnUpdate() => Entities.ForEach( ( ref TileUnderMouse tileUnderMouse, ref MouseWorldPosition mouseWorldPosition ) =>
		{
			if ( EntityManager.Exists( MapSettingsEntity ) )
			{
				MapSettings = EntityManager.GetSharedComponentData<MapSettings>( MapSettingsEntity );
			}

			if ( MapSettings.Tiles.Length != 0 )
			{
				tileUnderMouse.Tile = MapSettings.Tiles[IndexUtils.WorldIndex1D( mouseWorldPosition.Position, MapSettings.Tiles.Length )];
			}
			else
			{
				tileUnderMouse.Tile = new Entity();
			}
		} );
	}
}