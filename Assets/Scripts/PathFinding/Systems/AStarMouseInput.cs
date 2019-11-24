using Input.Components;

using PathFinding.Components;

using Unity.Entities;

using static Helpers.IndexUtils;

namespace PathFinding.Systems
{
	public class AStarMouseInput : ComponentSystem
	{
		private EntityArchetype requestArchetype;

		protected override void OnCreate()
		{
			EntityManager.CreateEntity( typeof( PathRequestBuilder ), typeof( MouseButtons ), typeof( MouseWorldPosition ) );
			requestArchetype = EntityManager.CreateArchetype( typeof( PathRequest ) );
		}

		protected override void OnUpdate() =>
			Entities.ForEach( ( ref PathRequestBuilder builder, ref MouseButtons mouseButtons, ref MouseWorldPosition mouseWorldPosition ) =>
			  {
				  if ( mouseButtons.Previous.HasFlag( MouseButton.Left ) && ( mouseButtons.Current.HasFlag( MouseButton.Left ) == false ) )
				  {
					  var tileIndex = WorldIndex2D( mouseWorldPosition.Position);
					  if ( builder.Started )
					  {
						  var entity = PostUpdateCommands.CreateEntity( requestArchetype );
						  PostUpdateCommands.SetComponent( entity, new PathRequest( builder.Start, tileIndex ) );
						  builder.Started = false;
					  }
					  else
					  {
						  builder.Started = true;
						  builder.Start = tileIndex;
					  }
				  }
			  } );
	}
}