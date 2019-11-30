using Helpers;
using Helpers.Assets.Scripts.Helpers;

using Maps.Authoring;
using Maps.Components;
using Maps.Systems;

using NUnit.Framework;

using PathFinding.Components;
using PathFinding.Systems;
using Tests;
using Unity.Collections;
using Unity.Mathematics;
using Unity.PerformanceTesting;

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


		}

		private void CreateRequest( int mapSize )
		{
			var request = _entityManager.CreateEntity( typeof( PathRequest ) );
			_entityManager.SetComponentData( request, new PathRequest() { Start = new int2( 0, 0 ), End = new int2( mapSize - 1, mapSize - 1 ) } );
		}

		private void CreateMap( int mapSize )
		{
			var AllTileSO = ResourcePath.TILES_SO.All<TileTypeSO>();

			// Create request
			var requestEntity = _entityManager.CreateEntity(typeof(MapRequest), typeof(MapSettings));
			var tileTypes = new BlitableArray<TileType>( AllTileSO.Length, Allocator.TempJob );

			for ( int i = 0; i < AllTileSO.Length; i++ )
			{
				tileTypes[i] = new TileType() { TileTypeSO = AllTileSO[i] };
			}

			_entityManager.SetSharedComponentData( requestEntity, new MapRequest() { Frequency = new float2( 0.1f, 0.1f ), TileTypes = tileTypes } );
			_entityManager.SetComponentData( requestEntity, new MapSettings() { CanMoveDiagonally = true, MapSize = mapSize } );

			var mapSpawnerSystem = _currentWorld.GetOrCreateSystem<MapSpawner>();
			mapSpawnerSystem.Update();

			var aStarNeighbours = _currentWorld.GetOrCreateSystem<AStarNeighbours>();
			aStarNeighbours.Update();
		}
	}
}