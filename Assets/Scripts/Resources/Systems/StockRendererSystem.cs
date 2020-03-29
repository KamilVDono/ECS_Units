using Maps.Components;

using Resources.Components;

using Unity.Entities;

using Visuals.Systems;

namespace Resources.Systems
{
	public struct HasStockRenderer : MeshCreatorSystemVisualComponent
	{
		public Entity VisualEntity;

		public Entity ValueEntity => VisualEntity;
		public bool IsValid => true;
	}

	[UpdateInGroup( typeof( PresentationSystemGroup ) )]
	public class StockRendererSystem : MeshCreatorSystem<HasStockRenderer, Stock>
	{
		protected override float Extends => 0.25f;

		protected override void OnUpdate()
		{
			base.OnUpdate();

			Entities
				.WithNone<HasStockRenderer>()
				.ForEach(
				( Entity entity, ref Stock stock, ref MapIndex mapIndex ) =>
				{
					var visualEntity = CreateVisualEntity( "Map/TileWave", stock.Type.Value.Color, ref mapIndex, 1 );
					EntityManager.AddComponentData( entity, new HasStockRenderer { VisualEntity = visualEntity } );
				} );
		}
	}
}