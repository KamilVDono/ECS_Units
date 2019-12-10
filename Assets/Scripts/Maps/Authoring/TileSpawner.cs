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
		[SF] private float2 _frequency;
		[SF] private float2 _offset;
		[SF] private int _mapSize;
		[SF] private TileTypeSO[] _tileTypes;

		private void Awake()
		{
			var entityManager = World.Active.EntityManager;

			Entity e = entityManager.CreateEntity( typeof(MapRequest) );

			var request = new MapRequest(){ Frequency = _frequency, Offset = _offset, TileTypes = new BlitableArray<TileType>(_tileTypes.Length, Allocator.TempJob), MapEdgeSize = _mapSize };
			for ( int i = 0; i < _tileTypes.Length; i++ )
			{
				TileTypeSO tileType = _tileTypes[i];
				request.TileTypes[i] = new TileType( tileType );
			}

			entityManager.SetSharedComponentData( e, request );
		}
	}
}