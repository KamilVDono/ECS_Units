using Helpers.Types;

using Maps.Components;

using Resources.Components;

using System.Collections.Generic;

using Unity.Entities;

using UnityEngine;

using Visuals.Systems;

namespace Resources.Systems
{
	public struct HasResourceOreRenderer : MeshCreatorSystemVisualComponent
	{
		#region Fields
		public Entity VisualEntity;
		public Boolean Valid;
		#endregion Fields

		#region MeshCreatorSystemVisualComponent
		public Entity ValueEntity => VisualEntity;
		public bool IsValid => Valid;
		#endregion MeshCreatorSystemVisualComponent
	}

	[UpdateInGroup( typeof( PresentationSystemGroup ) )]
	public class ResourceOreRendererSystem : MeshCreatorSystem<HasResourceOreRenderer, ResourceOre>
	{
		private readonly Dictionary<Color32, Material> _oreMaterials = new Dictionary<Color32, Material>();

		protected override float Extends => 0.35f;

		protected override void OnUpdate()
		{
			base.OnUpdate();

			var hasResourceOreRenderers = GetComponentDataFromEntity<HasResourceOreRenderer>();
			Entities
				.ForEach( ( Entity changeProvider, ref ResourceOreChange resourceOreChange ) =>
				{
					PostUpdateCommands.DestroyEntity( changeProvider );
					var oreEntity = resourceOreChange.oreEntity;

					if ( hasResourceOreRenderers.Exists( oreEntity ) )
					{
						var visual = hasResourceOreRenderers[oreEntity];
						if ( visual.Valid )
						{
							PostUpdateCommands.DestroyEntity( visual.VisualEntity );
						}
						PostUpdateCommands.RemoveComponent<HasResourceOreRenderer>( oreEntity );
					}
				} );

			Entities
				.WithNone<HasResourceOreRenderer>()
				.ForEach( ( Entity entity, ref ResourceOre ore, ref MapIndex mapIndex ) =>
				{
					var visualEntity = new Entity();
					if ( ore.IsValid )
					{
						visualEntity = CreateVisualEntity( "Map/Tile", ore.Type.Value.Color, ref mapIndex, 1 );
					}

					PostUpdateCommands.AddComponent( entity, new HasResourceOreRenderer { VisualEntity = visualEntity, Valid = ore.IsValid } );
				} );
		}
	}
}