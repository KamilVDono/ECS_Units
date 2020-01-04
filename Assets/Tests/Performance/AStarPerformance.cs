using Blobs;

using Maps.Authoring;
using Maps.Components;
using Maps.Systems;

using NUnit.Framework;

using Pathfinding.Components;
using Pathfinding.Systems;

using Tests;

using Unity.Mathematics;
using Unity.PerformanceTesting;

using UnityEngine;

namespace Performance
{
	public class AStarPerformance : ECSSystemTester<AStar>
	{
		[Test]
		[Performance]
		[Version( "1" )]
		public void AStarSimplePasses()
		{
			SampleGroupDefinition[] markers =
			{
				new SampleGroupDefinition("AStar.System"),
				new SampleGroupDefinition("AStar.Setup"),
				new SampleGroupDefinition("AStar.Search"),
				new SampleGroupDefinition("AStar.Reconstruct"),
				new SampleGroupDefinition("AStar.Cleanup"),
				new SampleGroupDefinition("AStar.Find_current" ),
				new SampleGroupDefinition("AStar.Movement_data"),
				new SampleGroupDefinition("AStar.Setup_data")
			};

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
			var requestEntity = _entityManager.CreateEntity(typeof(MapRequest), typeof(MapSettings));

			_entityManager.SetSharedComponentData( requestEntity, new MapRequest() { Frequency = new float2( 0.1f, 0.1f ) } );
			_entityManager.SetComponentData( requestEntity, new MapSettings() { MapEdgeSize = mapSize } );

			var mapSpawnerSystem = _currentWorld.GetOrCreateSystem<MapSpawner>();
			mapSpawnerSystem.Update();
		}
	}
}