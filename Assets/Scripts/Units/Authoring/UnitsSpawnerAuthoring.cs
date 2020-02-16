using Units.Components;

using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

using SF = UnityEngine.SerializeField;

namespace Units.Authoring
{
	public class UnitsSpawnerAuthoring : MonoBehaviour
	{
		[SF] private int _unitsCount;
		[SF] private int2 _texturesRowsColumns;
		[SF] private float _movementSpeed;
		[SF] private Material _unitMaterial;

		private void Awake()
		{
			var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

			Entity e = entityManager.CreateEntity( typeof(UnitsRequest) );

			var request = new UnitsRequest()
			{
				UnitsCount = _unitsCount,
				UnitSpeed = _movementSpeed,
				TextureTiles = _texturesRowsColumns,
			};

			entityManager.SetComponentData( e, request );
			entityManager.AddComponentObject( e, _unitMaterial );
		}
	}
}