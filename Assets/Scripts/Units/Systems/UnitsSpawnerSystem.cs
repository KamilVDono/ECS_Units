using Helpers;

using Maps.Components;
using Maps.Systems;

using Rendering.Components;

using Resources.Components;

using StateMachine.Components;

using System;

using Units.Components;
using Units.Components.Stats;

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
	public class UnitsSpawnerSystem : SystemBase
	{
		private const float EXTENDS = 0.5f;

		private EntityArchetype _unitArchetype;
		private Mesh _unitMesh;

		protected override void OnCreate()
		{
			RequireSingletonForUpdate<MapSettings>();
			_unitArchetype = EntityManager.CreateArchetype(
				// Custom
				typeof( UnitTag ), typeof( MovementSpeed ), typeof( MiningSpeed ), typeof( MapIndex ),
				// FSM
				typeof( IdleTag ),
				// 3D properties
				typeof( LocalToWorld ), typeof( Translation ), typeof( Rotation ),
				// Rendering
				typeof( RenderMesh ), typeof( RenderBounds ), typeof( TileMaterialProperty ), typeof( AnimationSpeedMaterialProperty ),
				typeof( RowsColumns_Tex_AnimMaterialProperty )
				);

			_unitMesh = MeshCreator.Quad( EXTENDS, quaternion.Euler( new float3( math.radians( 90 ), 0, 0 ) ) );
		}

		protected override void OnUpdate()
		{
			var mapSettings = GetSingleton<MapSettings>();
			var resourceOres = GetComponentDataFromEntity<ResourceOre>( true );

			var seed = (uint)DateTime.Now.GetHashCode();
			// Init random
			Random random = new Random();
			random.InitState( seed );

			Entities
				.WithReadOnly( resourceOres )
				.WithStructuralChanges()
				.ForEach( ( Entity e, in UnitsRequest unitsRequest ) =>
			{
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
					// Unit specific
					EntityManager.SetComponentData( entities[i], new MovementSpeed() { Speed = (float)unitsRequest.UnitSpeed } );
					EntityManager.SetComponentData( entities[i], new MiningSpeed() { Speed = (float)unitsRequest.UnitMiningSpeed } );

					// Position
					var mapIndex = freePositions[i];
					EntityManager.SetComponentData( entities[i], mapIndex );

					var position = new float3(mapIndex.Index2D.x, 3, mapIndex.Index2D.y);
					EntityManager.SetComponentData( entities[i], new Translation() { Value = position } );

					// Rendering
					EntityManager.SetSharedComponentData( entities[i], new RenderMesh { material = unitMaterial, mesh = _unitMesh } );
					EntityManager.SetComponentData( entities[i], new RenderBounds
					{
						Value = new AABB()
						{
							Center = float3.zero,
							Extents = new float3( EXTENDS, 0, EXTENDS )
						}
					} );
					var materialTile = new float4( random.NextInt(0, unitsRequest.TextureTiles.x), random.NextInt(0, unitsRequest.TextureTiles.y), 0, 0 );
					EntityManager.SetComponentData( entities[i], new TileMaterialProperty { Tile = materialTile } );
					EntityManager.SetComponentData( entities[i], new AnimationSpeedMaterialProperty { Speed = 0.5f } );
					EntityManager.SetComponentData( entities[i], new RowsColumns_Tex_AnimMaterialProperty { Value = new float4( 8, 12, 0, 3 ) } );
				}

				entities.Dispose();
				freePositionsSet.Dispose();
				freePositions.Dispose();
				EntityManager.DestroyEntity( e );
			} ).Run();
		}
	}
}