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
		#region Private fields
		[SF] private float2 _frequency;
		[SF] private float2 _offset;
		[SF] private int _mapEdgeSize;
		[SF] private TileTypeSO[] _tileTypes;
		#endregion Private fields

		#region Properties
		public int MapEdgeSize => _mapEdgeSize;
		#endregion Properties


		private void Awake()
		{
			foreach ( var tile in _tileTypes )
			{
				tile.SetupToString();
			}

			var entityManager = World.Active.EntityManager;

			Entity e = entityManager.CreateEntity( typeof(MapRequest) );

			var request = new MapRequest(){ Frequency = _frequency, Offset = _offset, TileTypes = new BlitableArray<GroundType>(_tileTypes.Length, Allocator.TempJob), MapEdgeSize = _mapEdgeSize };
			for ( int i = 0; i < _tileTypes.Length; i++ )
			{
				TileTypeSO tileType = _tileTypes[i];
				request.TileTypes[i] = new GroundType( tileType );
			}

			entityManager.SetSharedComponentData( e, request );
		}
	}
}