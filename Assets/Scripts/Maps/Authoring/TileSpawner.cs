using Helpers;

using Maps.Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

using SF = UnityEngine.SerializeField;

namespace Maps.Authoring
{
	public class TileSpawner : MonoBehaviour
	{
		[SF] private bool _canMoveDiagonal = true;
		[SF] private float2 _frequency;
		[SF] private int _mapSize;
		[SF] private TileTypeSO[] _tileTypes;

		private void Awake()
		{
			var entityManager = World.Active.EntityManager;

			var tileArchetype = entityManager.CreateArchetype(
				typeof(MapSettings),
				typeof(MapRequest)
			);

			Entity e = entityManager.CreateEntity( tileArchetype );

			var request = new MapRequest(){ Frequency = _frequency, TileTypes = new BlitableArray<TileType>(_tileTypes.Length, Allocator.TempJob) };
			for ( int i = 0; i < _tileTypes.Length; i++ )
			{
				TileTypeSO tileType = _tileTypes[i];
				request.TileTypes[i] = new TileType() { TileTypeSO = tileType };
			}

			var settings = new MapSettings(){ MapSize = _mapSize, CanMoveDiagonally = _canMoveDiagonal};

			entityManager.SetSharedComponentData( e, request );
			entityManager.SetComponentData( e, settings );
		}
	}
}