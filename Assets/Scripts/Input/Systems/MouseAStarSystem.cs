using Input.Components;

using Pathfinding.Components;

using Unity.Entities;

using static Helpers.IndexUtils;

namespace Input.Systems
{
	public class MouseAStarSystem : SystemBase
	{
		private EntityArchetype _requestArchetype;

		protected override void OnCreate()
		{
			RequireSingletonForUpdate<CurrentMouseMode>();
			EntityManager.CreateEntity( typeof( PathRequestBuilder ), typeof( MouseButtons ), typeof( MouseWorldPosition ) );
			_requestArchetype = EntityManager.CreateArchetype( typeof( PathRequest ), typeof( MouseRequestedPath ) );
		}

		protected override void OnUpdate()
		{
			if ( GetSingleton<CurrentMouseMode>().Mode != MouseMode.PathFinding )
			{
				return;
			}

			Entities
				.WithStructuralChanges()
				.WithoutBurst()
				.ForEach( ( ref PathRequestBuilder builder, ref MouseButtons mouseButtons, ref MouseWorldPosition mouseWorldPosition ) =>
			{
				if ( mouseButtons.Previous.HasFlag( MouseButton.Left ) && ( mouseButtons.Current.HasFlag( MouseButton.Left ) == false ) )
				{
					var tileIndex = WorldIndex2D( mouseWorldPosition.Position);
					if ( builder.Started )
					{
						var entity = EntityManager.CreateEntity( _requestArchetype );
						SetComponent( entity, new PathRequest( builder.Start, tileIndex ) );
						builder.Started = false;
					}
					else
					{
						builder.Started = true;
						builder.Start = tileIndex;
					}
				}
			} ).Run();
		}

		public struct MouseRequestedPath : IComponentData { }
	}
}