using Input.Components;

using Maps.Systems;

using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Input.Systems
{
	[UpdateInGroup( typeof( InitializationSystemGroup ) )]
	[UpdateAfter( typeof( MapSpawner ) )]
	public class MouseInputFeeder : ComponentSystem
	{
		private float2 _screenSize;

		protected override void OnCreate() => _screenSize = new float2( Screen.width, Screen.height );

		protected override void OnUpdate()
		{
			var unityMousePosition = Mouse.current.position.ReadValue();
			var mousePosition = new int2((int)unityMousePosition.x, (int)unityMousePosition.y);

			if ( mousePosition.x < 0 || mousePosition.y < 0 || mousePosition.x >= _screenSize.x || mousePosition.y >= _screenSize.y )
			{
				return;
			}

			Entities.ForEach( ( ref MouseScreenPosition mouseScreenPosition ) =>
			{
				var lastPos = mouseScreenPosition.Position;
				mouseScreenPosition.Position = mousePosition;
				mouseScreenPosition.Delta = mouseScreenPosition.Position - lastPos;
			} );

			var currentButtons = ExtractMouseButtons( );
			Entities.ForEach( ( ref MouseButtons mouseButtons ) =>
			{
				mouseButtons.Previous = mouseButtons.Current;
				mouseButtons.Current = currentButtons;
			} );

			Entities.ForEach( ( CameraData cameraData ) =>
			{
				var worldPosition = cameraData.Camera.ScreenToWorldPoint(new UnityEngine.Vector3(unityMousePosition.x, unityMousePosition.y, 0));
				var worldPosition2D = new float2(worldPosition.x, worldPosition.z);

				Entities.ForEach( ( ref MouseWorldPosition mouseWorldPosition ) =>
				{
					var lastPos = mouseWorldPosition.Position;
					mouseWorldPosition.Position = worldPosition2D;
					mouseWorldPosition.Delta = mouseWorldPosition.Position - lastPos;
				} );
			} );
		}

		private static MouseButton ExtractMouseButtons()
		{
			MouseButton currentButtons = MouseButton.None;
			if ( Mouse.current.leftButton.isPressed )
			{
				currentButtons |= MouseButton.Left;
			}
			if ( Mouse.current.middleButton.isPressed )
			{
				currentButtons |= MouseButton.Middle;
			}
			if ( Mouse.current.rightButton.isPressed )
			{
				currentButtons |= MouseButton.Right;
			}

			return currentButtons;
		}
	}
}