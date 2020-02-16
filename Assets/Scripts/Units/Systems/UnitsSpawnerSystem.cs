using Helpers;

using Maps.Components;
using Maps.Systems;

using Resources.Components;

using System.Collections.Generic;

using Units.Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

using UnityEngine;

using Random = Unity.Mathematics.Random;

namespace Units.Systems
{
	[UpdateInGroup( typeof( InitializationSystemGroup ) )]
	[UpdateAfter( typeof( MapSpawner ) )]
	public class UnitsSpawnerSystem : ComponentSystem // Just ComponentSystem because one time system
	{
		private EntityArchetype _unitArchetype;
		private Mesh _unitMesh;
		private Dictionary<int2, Material> _materials = new Dictionary<int2, Material>();

		protected override void OnCreate()
		{
			RequireSingletonForUpdate<MapSettings>();
			_unitArchetype = EntityManager.CreateArchetype(
				// Custom
				typeof( UnitTag ), typeof( MovementSpeed ), typeof( MapIndex ),
				// 3D properties
				typeof( LocalToWorld ),
				typeof( Translation ),
				typeof( Rotation ),
				// Rendering
				typeof( RenderMesh )
				);

			_unitMesh = MeshCreator.Quad( 0.45f, quaternion.Euler( new float3( math.radians( -90 ), 0, 0 ) ) );
		}

		protected override void OnUpdate()
		{
			var mapSettings = GetSingleton<MapSettings>();
			var resourceOres = GetComponentDataFromEntity<ResourceOre>( true );

			Entities.ForEach( ( Entity e, ref UnitsRequest unitsRequest ) =>
			{
				// Init random
				Random random = new Random();
				var seed = (uint)Time.ElapsedTime.ToString( "0.######" ).GetHashCode();
				random.InitState( seed == 0 ? 1 : seed );

				// Search for positions
				NativeHashMap<MapIndex, byte> freePositionsSet = new NativeHashMap<MapIndex, byte>( unitsRequest.UnitsCount, Allocator.Temp );
				for ( int i = 0; i < unitsRequest.UnitsCount; i++ )
				{
					var mapIndex = MapIndex.From(random.NextInt2(0, mapSettings.MapEdgeSize), mapSettings.MapEdgeSize);

					while ( resourceOres[mapSettings.Tiles[mapIndex.Index1D]].IsValid || freePositionsSet.ContainsKey( mapIndex ) )
					{
						mapIndex = MapIndex.From( random.NextInt2( 0, mapSettings.MapEdgeSize ), mapSettings.MapEdgeSize );
					}

					freePositionsSet.Add( mapIndex, 1 );
				}
				var freePositions = freePositionsSet.GetKeyArray( Allocator.Temp );

				// Spawn units
				var unitMaterial = EntityManager.GetComponentObject<Material>( e );
				NativeArray<Entity> entities = new NativeArray<Entity>(unitsRequest.UnitsCount, Allocator.Temp);
				EntityManager.CreateEntity( _unitArchetype, entities );

				for ( int i = 0; i < unitsRequest.UnitsCount; i++ )
				{
					PostUpdateCommands.SetComponent( entities[i], new MovementSpeed() { Speed = unitsRequest.UnitSpeed } );

					var mapIndex = freePositions[i];
					PostUpdateCommands.SetComponent( entities[i], mapIndex );

					var position = new float3(mapIndex.Index2D.x, 3, mapIndex.Index2D.y);
					PostUpdateCommands.SetComponent( entities[i], new Translation() { Value = position } );

					var materialTile = random.NextInt2(0, 32);
					PostUpdateCommands.SetSharedComponent( entities[i], new RenderMesh() { material = GetMaterial( materialTile, unitMaterial ), mesh = _unitMesh } );
				}

				entities.Dispose();
				freePositionsSet.Dispose();
				freePositions.Dispose();
				PostUpdateCommands.DestroyEntity( e );
			} );
		}

		private Material GetMaterial( int2 tileIndex, Material sourceMaterial )
		{
			if ( _materials.TryGetValue( tileIndex, out var material ) == false )
			{
				material = new Material( sourceMaterial );
				material.SetFloat( "_TileX", tileIndex.x );
				material.SetFloat( "_TileY", tileIndex.y );
				_materials[tileIndex] = material;
			}
			return material;
		}
	}
}