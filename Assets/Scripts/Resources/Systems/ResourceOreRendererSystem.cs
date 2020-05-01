using Helpers.Types;

using Maps.Components;

using Resources.Components;

using Unity.Entities;

using Visuals.Systems;

namespace Resources.Systems
{
	public struct HasResourceOreRenderer : MeshCreatorSystemVisualComponent
	{
		public Entity VisualEntity;
		public Boolean Valid;

		public Entity ValueEntity => VisualEntity;
		public bool IsValid => Valid;
	}

	[UpdateInGroup( typeof( PresentationSystemGroup ) )]
	public class ResourceOreRendererSystem : MeshCreatorSystem<HasResourceOreRenderer, ResourceOre>
	{
		protected override float Extends => 0.35f;

		protected override void OnUpdate()
		{
			base.OnUpdate();

			var hasResourceOreRenderers = GetComponentDataFromEntity<HasResourceOreRenderer>();

			// resource ore has zero count so remove
			Entities
				.ForEach( ( Entity changeProvider, ref ResourceOreChange resourceOreChange ) =>
				{
					var oreEntity = resourceOreChange.oreEntity;

					if ( hasResourceOreRenderers.Exists( oreEntity ) )
					{
						var visual = hasResourceOreRenderers[oreEntity];
						if ( visual.Valid )
						{
							PostUpdateCommands.DestroyEntity( visual.VisualEntity );
						}
					}

					PostUpdateCommands.DestroyEntity( changeProvider );
				} );

			// create new renderer for ore without renderer
			Entities
				.WithNone<HasResourceOreRenderer>()
				.ForEach( ( Entity entity, ref ResourceOre ore, ref MapIndex mapIndex ) =>
				{
					var visualEntity = new Entity();
					if ( ore.IsValid )
					{
						visualEntity = CreateVisualEntity( "Map/Tile", ore.Type.Value.Color, ref mapIndex, 1 );
					}

					EntityManager.AddComponentData( entity, new HasResourceOreRenderer { VisualEntity = visualEntity, Valid = ore.IsValid } );
				} );
		}
	}
}