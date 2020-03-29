using Helpers;

using Input.Components;

using Maps.Components;

using Units.Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Input.Systems
{
	public class MouseOverTileSystem : SystemBase
	{
		private EntityQuery _unitsQuery;

		protected override void OnCreate()
		{
			RequireSingletonForUpdate<MapSettings>();
			EntityManager.CreateEntity( typeof( TileUnderMouse ), typeof( UnitUnderMouse ), typeof( MouseWorldPosition ) );
			_unitsQuery = EntityManager.CreateEntityQuery( ComponentType.ReadOnly<UnitTag>(), ComponentType.ReadOnly<MapIndex>() );
		}

		protected override void OnUpdate()
		{
			var mapSettings = GetSingleton<MapSettings>();

			var unitsEntities = _unitsQuery.ToEntityArrayAsync( Allocator.TempJob, out var unitsJobHandle );
			var unitsMapIndexes = GetComponentDataFromEntity<MapIndex>( true );

			Entities
				.WithReadOnly( unitsEntities )
				.WithDeallocateOnJobCompletion( unitsEntities )
				.WithReadOnly( unitsMapIndexes )
				.ForEach( ( ref TileUnderMouse tileUnderMouse, ref UnitUnderMouse unitUnderMouse, in MouseWorldPosition mouseWorldPosition ) =>
				{
					var mouseIndex = IndexUtils.WorldIndex1D( mouseWorldPosition.Position, mapSettings.Tiles.Length );
					tileUnderMouse.Tile = mapSettings.Tiles[mouseIndex];

					Entity emptyEntity = new Entity();
					Entity unitEntity = new Entity();
					int i = 0;
					while ( emptyEntity == unitEntity && i < unitsEntities.Length )
					{
						var mapIndex = unitsMapIndexes[unitsEntities[i]];
						if ( mapIndex.Index1D == mouseIndex )
						{
							unitEntity = unitsEntities[i];
						}
						++i;
					}

					unitUnderMouse.Unit = unitEntity;
				} ).Schedule( unitsJobHandle );
		}
	}
}