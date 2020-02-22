using Helpers.Types;

using Units.Components;

using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

using SF = UnityEngine.SerializeField;

namespace Units.Authoring
{
	public class UnitsSpawnerAuthoring : MonoBehaviour
	{
		[SF] private int _unitsCount = 10;
		[SF] private int2 _texturesRowsColumns = new int2(8, 12);
		[SF] private FloatRange _movementSpeed = new FloatRange(0.8f, 2f);
		[SF] private FloatRange _miningSpeed = new FloatRange(1, 4);
		[SF] private Material _unitMaterial = null;

		private void Awake()
		{
			var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

			Entity e = entityManager.CreateEntity( typeof(UnitsRequest) );

			var request = new UnitsRequest()
			{
				UnitsCount = _unitsCount,
				UnitSpeed = _movementSpeed,
				UnitMiningSpeed = _miningSpeed,
				TextureTiles = _texturesRowsColumns,
			};

			entityManager.SetComponentData( e, request );
			entityManager.AddComponentObject( e, _unitMaterial );
		}
	}
}