using Maps.Components;

using Resources.Components;

using System.Collections.Generic;

using Unity.Entities;

using UnityEngine;

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
		private Dictionary<Color32, Material> _materials = new Dictionary<Color32, Material>();

		protected override float Extends => 0.25f;

		protected override void OnUpdate()
		{
			base.OnUpdate();

			Entities.WithNone<HasStockRenderer>().ForEach(
				( Entity entity, ref Stock stock, ref MapIndex mapIndex ) =>
				{
					var visualEntity = CreateVisualEntity( GetMaterial( stock.Type.Value.Color ), ref mapIndex, 1 );
					PostUpdateCommands.AddComponent( entity, new HasStockRenderer { VisualEntity = visualEntity } );
				} );
		}

		private Material GetMaterial( Color32 color )
		{
			if ( _materials.TryGetValue( color, out var material ) )
			{
				return material;
			}
			material = new Material( Shader.Find( "Map/TileWave" ) );
			material.SetColor( "_MainColor", color );
			material.enableInstancing = true;
			_materials.Add( color, material );
			return material;
		}
	}
}