using Helpers;

using Input.Components;

using Maps.Components;

using Resources.Components;

using Unity.Entities;

using Working.Components;

namespace Input.Systems
{
	public class MouseMiningSystem : ComponentSystem
	{
		protected override void OnCreate()
		{
			RequireSingletonForUpdate<MapSettings>();
			RequireSingletonForUpdate<CurrentMouseMode>();
			EntityManager.CreateEntity( typeof( MouseButtons ), typeof( MouseWorldPosition ), typeof( MouseMiningTag ) );
		}

		protected override void OnUpdate()
		{
			var mouseMode = GetSingleton<CurrentMouseMode>();
			if ( mouseMode.Mode != MouseMode.Mining )
			{
				return;
			}

			var mapSettings = GetSingleton<MapSettings>();
			var ores = GetComponentDataFromEntity<ResourceOre>(true);

			Entities
				.WithAll<MouseMiningTag>()
				.ForEach( ( ref MouseButtons mouseButtons, ref MouseWorldPosition mouseWorldPosition ) =>
				{
					if ( mouseButtons.IsClick( MouseButton.Left ) )
					{
						var clickedEntity = mapSettings.Tiles[IndexUtils.WorldIndex1D( mouseWorldPosition.Position, mapSettings.Tiles.Length )];
						var clickedOre = ores[clickedEntity];

						if ( clickedOre.IsValid )
						{
							if ( EntityManager.HasComponent<WorkProgress>( clickedEntity ) )
							{
								EntityManager.SetComponentData( clickedEntity, new WorkProgress() { Progress = clickedOre.Type.Value.WorkRequired } );
							}
							else
							{
								EntityManager.AddComponentData( clickedEntity, new WorkProgress() { Progress = clickedOre.Type.Value.WorkRequired } );
							}

							if ( EntityManager.HasComponent<MiningWork>( clickedEntity ) == false )
							{
								EntityManager.AddComponentData( clickedEntity, new MiningWork() { ProgressPerSecond = 0 } );
							}
						}
					}
					if ( mouseButtons.IsClick( MouseButton.Right ) )
					{
						var clickedEntity = mapSettings.Tiles[IndexUtils.WorldIndex1D( mouseWorldPosition.Position, mapSettings.Tiles.Length )];
						var clickedOre = ores[clickedEntity];

						if ( clickedOre.IsValid )
						{
							if ( EntityManager.HasComponent<WorkProgress>( clickedEntity ) == false )
							{
								EntityManager.AddComponentData( clickedEntity, new WorkProgress() { Progress = 0 } );
							}

							if ( EntityManager.HasComponent<MiningWork>( clickedEntity ) == false )
							{
								EntityManager.AddComponentData( clickedEntity, new MiningWork() { ProgressPerSecond = clickedOre.Type.Value.WorkRequired } );
							}
							else
							{
								EntityManager.SetComponentData( clickedEntity, new MiningWork() { ProgressPerSecond = clickedOre.Type.Value.WorkRequired } );
							}
						}
					}
				} );
		}

		internal struct MouseMiningTag : IComponentData { }
	}
}