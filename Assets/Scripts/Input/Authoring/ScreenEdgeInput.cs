using Unity.Mathematics;

using UnityEngine;

namespace Input.Authoring
{
	public class ScreenEdgeInput
	{
		private float4 _offsets;

		public ScreenEdgeInput( int2 screen, float offsetPercentage )
		{
			float xOffset = screen.x * offsetPercentage;
			float yOffset = screen.y * offsetPercentage;
			_offsets = new float4(
				xOffset, screen.x - xOffset,
				yOffset, screen.y - yOffset );
		}

		public Vector2 DigitalInput( Vector2 pointerPosition )
		{
			var output = Vector2.zero;

			if ( pointerPosition.x <= _offsets.x )
			{
				output.x = -1;
			}
			else if ( pointerPosition.x >= _offsets.y )
			{
				output.x = 1;
			}

			if ( pointerPosition.y <= _offsets.z )
			{
				output.y = -1;
			}
			else if ( pointerPosition.y >= _offsets.w )
			{
				output.y = 1;
			}

			return output;
		}

		public Vector2 AnalogInput( Vector2 pointerPosition )
		{
			var output = Vector2.zero;

			if ( pointerPosition.x <= _offsets.x )
			{
				output.x = Mathf.Max( ( pointerPosition.x / _offsets.x ) - 1f, -1f );
			}
			else if ( pointerPosition.x >= _offsets.y )
			{
				output.x = Mathf.Min( ( pointerPosition.x - _offsets.y ) / _offsets.x, 1f );
			}

			if ( pointerPosition.y <= _offsets.z )
			{
				output.y = Mathf.Max( ( pointerPosition.y / _offsets.z ) - 1f, -1f );
			}
			else if ( pointerPosition.y >= _offsets.w )
			{
				output.y = Mathf.Min( ( pointerPosition.y - _offsets.w ) / _offsets.z, 1f );
			}

			return output;
		}
	}
}