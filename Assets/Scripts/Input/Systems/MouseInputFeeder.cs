using Input.Components;

using Maps.Systems;

using System.Diagnostics;

using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Input.Systems
{
	[UpdateInGroup( typeof( InitializationSystemGroup ) )]
	[UpdateAfter( typeof( MapSpawner ) )]
	public class MouseInputFeeder : SystemBase
	{
		private float2 _screenSize;

		private EntityQuery _tileUnderMouseQuery;

		protected override void OnCreate()
		{
			_screenSize = new float2( Screen.width, Screen.height );
			_tileUnderMouseQuery = EntityManager.CreateEntityQuery( ComponentType.ReadOnly<CameraData>() );
		}

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
			} ).Run();

			var currentButtons = ExtractMouseButtons( );
			Entities.ForEach( ( ref MouseButtons mouseButtons ) =>
			{
				mouseButtons.Previous = mouseButtons.Current;
				mouseButtons.Current = currentButtons;
			} ).Run();

			ValidateCameraDataCount();

			var cameraDataEntities = _tileUnderMouseQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
			var cameraData = EntityManager.GetSharedComponentData<CameraData>(cameraDataEntities[0]);
			cameraDataEntities.Dispose();

			var worldPosition2D = cameraData.ScreenToWorldPoint2D(unityMousePosition);
			Entities.ForEach( ( ref MouseWorldPosition mouseWorldPosition ) =>
			{
				var lastPos = mouseWorldPosition.Position;
				mouseWorldPosition.Position = worldPosition2D;
				mouseWorldPosition.Delta = mouseWorldPosition.Position - lastPos;
			} ).Run();
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

		[Conditional( "DEBUG" )]
		private void ValidateCameraDataCount()
		{
			var count = _tileUnderMouseQuery.CalculateEntityCount();
			if ( count > 1 )
			{
				throw new System.Exception( $"There is more than one {typeof( CameraData )} components" );
			}
			if ( count < 1 )
			{
				throw new System.Exception( $"There is no {typeof( CameraData )} component" );
			}
		}
	}
}