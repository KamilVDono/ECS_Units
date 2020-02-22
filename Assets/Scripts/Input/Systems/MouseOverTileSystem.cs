using Helpers;

using Input.Components;

using Maps.Components;

using Units.Components.Tags;

using Unity.Entities;

namespace Input.Systems
{
	public class MouseOverTileSystem : ComponentSystem
	{
		protected override void OnCreate()
		{
			RequireSingletonForUpdate<MapSettings>();
			EntityManager.CreateEntity( typeof( TileUnderMouse ), typeof( UnitUnderMouse ), typeof( MouseWorldPosition ) );
		}

		protected override void OnUpdate()
		{
			var mapSettings = GetSingleton<MapSettings>();

			Entities
				.ForEach( ( ref TileUnderMouse tileUnderMouse, ref UnitUnderMouse unitUnderMouse, ref MouseWorldPosition mouseWorldPosition ) =>
				{
					var mouseIndex = IndexUtils.WorldIndex1D( mouseWorldPosition.Position, mapSettings.Tiles.Length );
					tileUnderMouse.Tile = mapSettings.Tiles[mouseIndex];

					Entity unitEntity = new Entity();
					Entities.WithAll<UnitTag>().ForEach( ( Entity unit, ref MapIndex mapIndex ) =>
					{
						if ( mapIndex.Index1D == mouseIndex )
						{
							unitEntity = unit;
						}
					} );

					unitUnderMouse.Unit = unitEntity;
				} );
		}
	}
}