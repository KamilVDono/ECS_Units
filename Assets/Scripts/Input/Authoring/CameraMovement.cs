using Maps.Authoring;

using UnityEngine;
using UnityEngine.InputSystem;

using UnityInput;

using SF = UnityEngine.SerializeField;

namespace Input.Authoring
{
	public class CameraMovement : MonoBehaviour
	{
		[SF] private Camera _camera;
		[SF] private Vector2 _speed;
		[SF] private float _screenInputOffset;
		[SF] private Vector2 _zoomBounds;
		[SF] private TileSpawner _tileSpawner;

		private CameraInput _input;
		private Bounds _mapBounds;
		private ScreenEdgeInput _screenInput;
		private Vector2Int _screenSize;
		private int _mapSize;

		private float _AspectRatio => _screenSize.x / (float)_screenSize.y;

		#region Unity Lifetime

		private void Awake()
		{
			SetupInitialState();
			CalculateCameraBounds();
		}


		private void SetupInitialState()
		{
			_input = new CameraInput();
			_screenSize = new Vector2Int( Screen.width, Screen.height );
		}

		private void CalculateCameraBounds()
		{
			_mapSize = _tileSpawner.MapEdgeSize;

			_mapBounds = CalculateBounds( _mapSize, _camera.orthographicSize, transform.position.y, _AspectRatio );
			transform.position += _mapBounds.center;

			_screenInput = new ScreenEdgeInput( new Unity.Mathematics.int2( _screenSize.x, _screenSize.y ), _screenInputOffset );

			if ( _mapSize / 2f < _zoomBounds.y * _AspectRatio )
			{
				_zoomBounds.y = ( _mapSize / 2f - 1f ) / _AspectRatio;
			}
		}

		private void LateUpdate()
		{
			UpdateZoom();
			UpdatePosition();
		}

		#region CameraInput lifetime

		private void OnEnable() => _input.Enable();

		private void OnDisable() => _input.Disable();

		private void OnDestroy() => _input.Dispose();

		#endregion CameraInput lifetime

		#endregion Unity Lifetime

		private void UpdateZoom()
		{
			var zoom = -_input.Camera.Zoom.ReadValue<float>() / 100f;
			if ( Mathf.Abs( zoom ) > 0.1f )
			{
				_camera.orthographicSize = Mathf.Clamp( _camera.orthographicSize + zoom, _zoomBounds.x, _zoomBounds.y );
				_mapBounds = CalculateBounds( _mapSize, _camera.orthographicSize, transform.position.y, _AspectRatio );
			}
		}

		private void UpdatePosition()
		{
			Vector2 input = _input.Camera.Move.ReadValue<Vector2>();

			input = input.sqrMagnitude < 0.01f ? _screenInput.DigitalInput( Mouse.current.position.ReadValue() ) : input;

			input *= _speed * Time.deltaTime;

			var position = transform.position + new Vector3( input.x, 0, input.y );

			transform.position = CheckBounds( _mapBounds, position );
		}

		#region Static helpers

		public static Bounds CalculateBounds( int mapSize, float viewportSize, float yPosition, float aspectRatio )
		{
			Bounds bounds = new Bounds
			{
				center = new Vector3( ( mapSize - 1 ) / 2f, yPosition, ( mapSize - 1 ) / 2f )
			};
			bounds.extents = new Vector3( bounds.center.x - viewportSize * aspectRatio + 0.5f, 0, bounds.center.z - viewportSize + 0.5f );

			if ( mapSize <= 0 )
			{
				throw new System.ArgumentException( $"{nameof( mapSize )} must be grater than zero" );
			}
			if ( viewportSize <= 0 )
			{
				throw new System.ArgumentException( $"{nameof( viewportSize )} must be grater than zero" );
			}
			if ( aspectRatio < 1 )
			{
				throw new System.ArgumentException( $"{nameof( aspectRatio )} must be grater or equal to one" );
			}
			if ( bounds.extents.x < 0 || bounds.extents.y < 0 )
			{
				throw new System.ArgumentOutOfRangeException( $"For {nameof( mapSize )}({mapSize}), {nameof( viewportSize )}({viewportSize}) and {nameof( aspectRatio )}({aspectRatio}) is impossible to produce valid bounds" );
			}
			return bounds;
		}

		public static Vector3 CheckBounds( Bounds mapBounds, Vector3 position ) => mapBounds.ClosestPoint( position );

		#endregion Static helpers
	}
}