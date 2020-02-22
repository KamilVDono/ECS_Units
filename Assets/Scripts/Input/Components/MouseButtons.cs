using System;

using Unity.Entities;

using Boolean = Helpers.Types.Boolean;

namespace Input.Components
{
	[Flags]
	public enum MouseButton : byte
	{
		None = 0,
		Left   = 1 << 0,
		Right  = 1 << 1,
		Middle = 1 << 2,
	}

	public struct MouseButtons : IComponentData
	{
		public MouseButton Previous;
		public MouseButton Current;

		/// <summary>
		/// Check if button become clicked within this frame
		/// </summary>
		public Boolean IsClick( MouseButton button )
		{
			// In previous frame button was not clicked but now is
			return ( ( Previous & button ) == 0 ) && ( ( Current & button ) != 0 );
		}

		/// <summary>
		/// Check if button become released within this frame
		/// </summary>
		public Boolean IsRelease( MouseButton button ) => ( ( Previous & button ) != 0 ) && ( ( Current & button ) == 0 );

		public Boolean IsDown( MouseButton button ) => ( ( Current & button ) != 0 );

		public Boolean IsUp( MouseButton button ) => ( ( Current & button ) == 0 );
	}
}