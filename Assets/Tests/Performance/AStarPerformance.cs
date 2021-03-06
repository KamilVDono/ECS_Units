﻿using Blobs;

using Maps.Authoring;
using Maps.Components;
using Maps.Systems;

using NUnit.Framework;

using Pathfinding.Components;
using Pathfinding.Systems;

using Tests.Utility;

using Unity.Mathematics;
using Unity.PerformanceTesting;

using UnityEngine;

namespace Performance
{
	public class AStarPerformance : ECSSystemTester<AStar>
	{
		private static readonly string[] markers =
			{
				"AStar.System",
				"AStar.Setup",
				"AStar.Search",
				"AStar.Reconstruct",
				"AStar.Cleanup",
				"AStar.Find_current",
				"AStar.Movement_data",
				"AStar.Setup_data",
				"AStar.Setup_data_movement",
				"AStar.Pop"
			};

		[Test]
		[Performance]
		[Version( "1" )]
		public void AStarSimplePasses()
		{
			Measure.Method( () =>
			{
				using ( Measure.ProfilerMarkers( markers ) )
				{
					CreateMap( 70 );
					CreateRequest( 70 );
					Update();
				}
			} )
				.WarmupCount( 10 )
				.MeasurementCount( 15 )
				.IterationsPerMeasurement( 6 )
				.Run();

			BlobsMemory.Instance.Dispose();
		}

		[Test]
		[Performance]
		[Version( "1" )]
		public void AStarHardPasses()
		{
			Measure.Method( () =>
			{
				using ( Measure.ProfilerMarkers( markers ) )
				{
					CreateMap( 400 );
					CreateRequest( 30 );
					Update();
				}
			} )
				.WarmupCount( 5 )
				.MeasurementCount( 10 )
				.IterationsPerMeasurement( 3 )
				.Run();

			BlobsMemory.Instance.Dispose();
		}

		private void CreateRequest( int mapSize )
		{
			var request = _entityManager.CreateEntity( typeof( PathRequest ) );
			_entityManager.SetComponentData( request, new PathRequest() { Start = new int2( 0, 0 ), End = new int2( mapSize - 1, mapSize - 1 ) } );
		}

		private void CreateMap( int mapSize )
		{
			var AllTileSO = new TileTypeSO[10];
			for ( int i = 0; i < AllTileSO.Length; i++ )
			{
				AllTileSO[i] = ScriptableObject.CreateInstance<TileTypeSO>();
				AllTileSO[i].name = i.ToString();
				AllTileSO[i].Cost = i * i;
				AllTileSO[i].Color = new Color32( (byte)i, (byte)i, (byte)i, 1 );
				AllTileSO[i].Range = 1f / AllTileSO.Length;
			}

			BlobsMemory.FromSOs( AllTileSO );

			// Create request
			var requestEntity = _entityManager.CreateEntity(typeof(MapRequest));

			_entityManager.SetComponentData( requestEntity, new MapRequest() { Frequency = new float2( 0.1f, 0.1f ), MapEdgeSize = mapSize } );

			var mapSpawnerSystem = _currentWorld.GetOrCreateSystem<MapSpawner>();
			mapSpawnerSystem.Update();

			var movementCostSystem = _currentWorld.GetOrCreateSystem<MovementCostTrackerSystem>();
			movementCostSystem.Update();
		}
	}
}