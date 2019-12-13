﻿using Helpers;

using Maps.Components;

using Pathfinding.Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

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
				typeof( RenderMesh ),
				typeof( TileType ),
				typeof( LocalToWorld ),
				typeof( Translation ),
				typeof( Rotation ),
				typeof( MovementCost ),
				typeof( MapLayer )
			);
			_mapSettingsArchetype = EntityManager.CreateArchetype( typeof( MapSettings ) );
		}

		protected override void OnUpdate() =>
			Entities.ForEach( ( Entity e, MapRequest mapRequest ) =>
		{
			int mapEdgeSize = mapRequest.MapEdgeSize;
			NativeArray<Entity> tileEntities = new NativeArray<Entity>(mapEdgeSize * mapEdgeSize, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			EntityManager.CreateEntity( _tileArchetype, tileEntities );
			var rotation = quaternion.Euler( new float3( math.radians(90), 0, 0 ) );

			MapLayer layer = new MapLayer{Layer = MapLayerType.Tile};

			for ( int y = 0; y < mapEdgeSize; y++ )
			{
				for ( int x = 0; x < mapEdgeSize; x++ )
				{
					var tileEntity = tileEntities[y * mapEdgeSize + x];
					var tileType = FindTileType( new float2( x, y ), mapRequest );
					PostUpdateCommands.SetSharedComponent( tileEntity, new RenderMesh { mesh = tileType.TileTypeBlob.Value.Mesh, material = tileType.TileTypeBlob.Value.Material } );
					PostUpdateCommands.SetSharedComponent( tileEntity, tileType );
					PostUpdateCommands.SetSharedComponent( tileEntity, layer );
					PostUpdateCommands.SetComponent( tileEntity, new Translation { Value = new float3( x, 0, y ) } );
					PostUpdateCommands.SetComponent( tileEntity, new Rotation { Value = rotation } );
					PostUpdateCommands.SetComponent( tileEntity, new MovementCost { Cost = tileType.TileTypeBlob.Value.MoveCost } );
				}
			}

			mapRequest.TileTypes.Dispose();
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

		private TileType FindTileType( float2 position, MapRequest mapRequest )
		{
			var tileTypes = mapRequest.TileTypes;

			float perlin = math.remap(-1, 1, 0, 1, noise.snoise((position * mapRequest.Frequency) + mapRequest.Offset));

			int index = 0;

			while ( index < tileTypes.Length && perlin - tileTypes[index].TileTypeBlob.Value.NoiseRange > 0 )
			{
				perlin -= tileTypes[index].TileTypeBlob.Value.NoiseRange;
				index++;
			}

			index = math.clamp( index, 0, tileTypes.Length - 1 );
			return mapRequest.TileTypes[index];
		}

		#endregion Methods
	}
}