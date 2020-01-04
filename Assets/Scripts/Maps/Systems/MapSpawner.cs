using Blobs;

using Helpers;

using Maps.Components;

using Pathfinding.Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Maps.Systems
{
	[UpdateInGroup( typeof( InitializationSystemGroup ) )]
	public class MapSpawner : ComponentSystem
	{
		private EntityArchetype _tileArchetype;
		private EntityArchetype _mapSettingsArchetype;

		#region Methods

		protected override void OnCreate()
		{
			_tileArchetype = EntityManager.CreateArchetype(
				// Map data
				typeof( GroundType ),
				typeof( MapIndex ),
				// A* data
				typeof( MovementCost )
			);
			_mapSettingsArchetype = EntityManager.CreateArchetype( typeof( MapSettings ) );
		}

		protected override void OnUpdate() =>
			Entities.ForEach( ( Entity e, MapRequest mapRequest ) =>
		{
			int mapEdgeSize = mapRequest.MapEdgeSize;
			NativeArray<Entity> tileEntities = new NativeArray<Entity>(mapEdgeSize * mapEdgeSize, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			EntityManager.CreateEntity( _tileArchetype, tileEntities );


			int index1D = 0;
			for ( int y = 0; y < mapEdgeSize; y++ )
			{
				for ( int x = 0; x < mapEdgeSize; x++ )
				{
					var tileEntity = tileEntities[y * mapEdgeSize + x];
					var tileType = FindTileType( new float2( x, y ), mapRequest );
					PostUpdateCommands.SetSharedComponent( tileEntity, tileType );
					PostUpdateCommands.SetComponent( tileEntity, new MapIndex( index1D, new int2( x, y ) ) );

					++index1D;
				}
			}

			PostUpdateCommands.RemoveComponent( e, typeof( MapRequest ) );
			PostUpdateCommands.DestroyEntity( e );

			var mapSettingsEntity = PostUpdateCommands.CreateEntity( _mapSettingsArchetype );

			var tiles = new BlitableArray<Entity>();
			tiles.Allocate( tileEntities, Allocator.Persistent );
			PostUpdateCommands.SetComponent( mapSettingsEntity, new MapSettings { MapEdgeSize = mapEdgeSize, Tiles = tiles } );

			tileEntities.Dispose();
		} );

		protected override void OnDestroy() =>
			Entities.ForEach( ( ref MapSettings mapSetting ) => mapSetting.Tiles.Dispose() );

		private GroundType FindTileType( float2 position, MapRequest mapRequest )
		{
			var tileTypes = BlobsMemory.Instance.ReferencesOf<GroundTypeBlob>();

			float perlin = math.remap(-1, 1, 0, 1, noise.snoise((position * mapRequest.Frequency) + mapRequest.Offset));

			int index = 0;

			while ( index < tileTypes.Length && perlin - tileTypes[index].Value.NoiseRange > 0 )
			{
				perlin -= tileTypes[index].Value.NoiseRange;
				index++;
			}

			index = math.clamp( index, 0, tileTypes.Length - 1 );
			return new GroundType( tileTypes[index] );
		}

		#endregion Methods
	}
}