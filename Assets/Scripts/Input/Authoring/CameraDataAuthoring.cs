using Input.Components;

using Unity.Entities;

using UnityEngine;

using SF = UnityEngine.SerializeField;

namespace Input.Authoring
{
	public class CameraDataAuthoring : MonoBehaviour
	{
		[SF] private Camera _camera = null;

		private void Start()
		{
			var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			var cameraEntity = entityManager.CreateEntity(typeof(CameraData));
			entityManager.SetSharedComponentData<CameraData>( cameraEntity, new CameraData { Camera = _camera } );
			entityManager.SetName( cameraEntity, "Camera data" );
			Destroy( this );
		}
	}
}