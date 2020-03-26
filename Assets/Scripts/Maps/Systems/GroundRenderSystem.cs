using Maps.Components;

using Unity.Entities;

using Visuals.Systems;

namespace Maps.Systems
{
	public struct HasGroundRenderer : MeshCreatorSystemVisualComponent
	{
		public Entity VisualEntity;

		public Entity ValueEntity => VisualEntity;
		public bool IsValid => true;
	}

	[UpdateInGroup( typeof( PresentationSystemGroup ) )]
	public class GroundRenderSystem : MeshCreatorSystem<HasGroundRenderer, GroundType>
	{
		protected override float Extends => 0.5f;

		protected override void OnUpdate()
		{
			base.OnUpdate();

			Entities.WithNone<HasGroundRenderer>().ForEach(
				( Entity entity, ref GroundType groundType, ref MapIndex mapIndex ) =>
				{
					var visualEntity = CreateVisualEntity( groundType.TileTypeBlob.Value.ShaderName.ToString(),
						groundType.TileTypeBlob.Value.MainColor, ref mapIndex, 0 );
					PostUpdateCommands.AddComponent( entity, new HasGroundRenderer { VisualEntity = visualEntity } );
				} );
		}
	}
}