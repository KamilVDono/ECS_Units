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
			var commandBuffer = World.DefaultGameObjectInjectionWorld.GetExistingSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
			var archetype = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(typeof( CameraData ));
			var entity = commandBuffer.CreateEntity( archetype );
			commandBuffer.SetSharedComponent( entity, new CameraData() { Camera = _camera } );
			Destroy( this );
		}
	}
}