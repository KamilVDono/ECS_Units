using Maps.Components;

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
		#endregion Private fields

		#region Properties
		public int MapEdgeSize => _mapEdgeSize;
		#endregion Properties

		private void Awake()
		{
			var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

			Entity e = entityManager.CreateEntity( typeof(MapRequest) );

			var request = new MapRequest()
			{
				Frequency = _frequency,
				Offset = _offset,
				MapEdgeSize = _mapEdgeSize
			};

			entityManager.SetSharedComponentData( e, request );
		}
	}
}