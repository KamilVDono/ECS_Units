using Helpers;

using Maps.Components;

using PathFinding.Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Maps.Systems
{
	public class MapSpawner : ComponentSystem
	{
		#region Private Fields
		private EntityArchetype _tileArchetype;
		#endregion Private Fields

		#region Protected Methods

		protected override void OnCreate() => _tileArchetype = EntityManager.CreateArchetype(
				typeof( RenderMesh ),
				typeof( TileType ),
				typeof( LocalToWorld ),
				typeof( Translation ),
				typeof( Rotation ),
				typeof( MovementCost )
			);

		protected override void OnDestroy() => Entities.ForEach( ( ref MapSettings mapSetting ) => mapSetting.Tiles.Dispose() );

		protected override void OnUpdate() => Entities.ForEach( ( Entity e, MapRequest mapRequest, ref MapSettings settings ) =>
		{
			var mapSize = settings.MapSize;
			NativeArray<Entity> tileEntities =  new NativeArray<Entity>(mapSize * mapSize, Allocator.Temp);
			EntityManager.CreateEntity( _tileArchetype, tileEntities );
			var rotation = quaternion.Euler( new float3( math.radians(90), 0, 0 ) );

			for ( int y = 0; y < mapSize; y++ )
			{
				for ( int x = 0; x < mapSize; x++ )
				{
					var tileEntity = tileEntities[y * mapSize + x];
					var tileType = FindTileType( new float2( x, y ), mapRequest );
					PostUpdateCommands.SetSharedComponent( tileEntity, new RenderMesh { mesh = tileType.TileTypeSO.Mesh, material = tileType.TileTypeSO.Material } );
					PostUpdateCommands.SetSharedComponent( tileEntity, tileType );
					PostUpdateCommands.SetComponent( tileEntity, new Translation { Value = new float3( x, 0, y ) } );
					PostUpdateCommands.SetComponent( tileEntity, new Rotation { Value = rotation } );
					PostUpdateCommands.SetComponent( tileEntity, new MovementCost { Cost = tileType.TileTypeSO.Cost } );
				}
			}

			mapRequest.TileTypes.Dispose();
			PostUpdateCommands.RemoveComponent( e, typeof( MapRequest ) );

			settings.Tiles = new BlitableArray<Entity>();
			settings.Tiles.Allocate( tileEntities, Allocator.Persistent );

			tileEntities.Dispose();
		} );

		#endregion Protected Methods

		#region Private Methods

		private TileType FindTileType( float2 position, MapRequest mapRequest )
		{
			var tileTypes = mapRequest.TileTypes;

			float perlin = math.remap(-1, 1, 0, 1, noise.snoise(position * mapRequest.Frequency));

			int index = 0;

			while ( index < tileTypes.Length && perlin - tileTypes[index].TileTypeSO.Range > 0 )
			{
				perlin -= tileTypes[index].TileTypeSO.Range;
				index++;
			}

			index = math.clamp( index, 0, tileTypes.Length - 1 );
			return mapRequest.TileTypes[index];
		}

		#endregion Private Methods
	}
}