using Input.Components;

using Unity.Entities;

using UnityEngine;

using SF = UnityEngine.SerializeField;

namespace Scripts.Assets.Scripts.Input.Authoring
{
	public class CameraDataAuthoring : MonoBehaviour
	{
		[SF] private Camera _camera;

		private void Start()
		{
			var commandBuffer = World.Active.GetExistingSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
			var archetype = World.Active.EntityManager.CreateArchetype(typeof( CameraData ));
			var entity = commandBuffer.CreateEntity( archetype );
			commandBuffer.SetSharedComponent( entity, new CameraData() { Camera = _camera } );
			Destroy( this );
		}
	}
}