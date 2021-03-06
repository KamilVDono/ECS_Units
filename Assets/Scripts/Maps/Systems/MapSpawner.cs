﻿using Blobs;

using Helpers;
using Helpers.Types;

using Maps.Components;

using Pathfinding.Components;

using Resources.Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Maps.Systems
{
	[UpdateInGroup( typeof( InitializationSystemGroup ) )]
	public class MapSpawner : SystemBase
	{
		private EntityArchetype _tileArchetype;
		private EntityArchetype _mapSettingsArchetype;

		protected override void OnCreate()
		{
			_tileArchetype = EntityManager.CreateArchetype(
				// Map data
				typeof( GroundType ),
				typeof( MapIndex ),
				// Resources
				typeof( ResourceOre ),
				// A* data
				typeof( MovementCost )
			);
			_mapSettingsArchetype = EntityManager.CreateArchetype( typeof( MapSettings ) );
		}

		protected override void OnUpdate()
		{
			Entities
				.WithStructuralChanges()
				.ForEach( ( Entity e, in MapRequest mapRequest ) =>
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
						var resourceOre = CalcResourceOre( new float2( x, y ), mapRequest, tileType );
						EntityManager.SetComponentData( tileEntity, tileType );
						EntityManager.SetComponentData( tileEntity, new MapIndex( index1D, new int2( x, y ) ) );
						EntityManager.SetComponentData( tileEntity, resourceOre );

						++index1D;
					}
				}

				EntityManager.DestroyEntity( e );

				var tiles = new BlitableArray<Entity>();
				tiles.Allocate( tileEntities, Allocator.Persistent );
				var mapSettingsEntity = EntityManager.CreateEntity( _mapSettingsArchetype );
				SetSingleton( new MapSettings { MapEdgeSize = mapEdgeSize, Tiles = tiles } );

				tileEntities.Dispose();

				Enabled = false;
			} ).Run();
		}

		protected override void OnDestroy()
		{
			if ( HasSingleton<MapSettings>() )
			{
				GetSingleton<MapSettings>().Tiles.Dispose();
			}
		}

		#region Methods

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

		private ResourceOre CalcResourceOre( float2 position, MapRequest mapRequest, GroundType tileType )
		{
			float perlin = math.remap(-1, 1, 0, 1, noise.snoise((position * mapRequest.Frequency) + mapRequest.Offset));
			if ( perlin > 0.3f && tileType.TileTypeBlob.Value.AcceptResourceOre )
			{
				return new ResourceOre() { Capacity = 100, Count = 100, Type = BlobsMemory.Instance.ReferencesOf<ResourceTypeBlob>().RandomPick() };
			}

			return ResourceOre.EMPTY_ORE;
		}

		#endregion Methods
	}
}