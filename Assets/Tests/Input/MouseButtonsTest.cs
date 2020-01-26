using Input.Components;

using NUnit.Framework;

namespace Tests.Input
{
	public class MouseButtonsTest
	{
		public class Single_Current
		{
			[Test]
			public void Left()
			{
				MouseButtons buttons = new MouseButtons()
				{
					Previous = MouseButton.None,
					Current = MouseButton.Left
				};

				var button = MouseButton.Left;
				Assert.IsTrue( buttons.IsClick( button ) );
				Assert.IsFalse( buttons.IsRelease( button ) );
				Assert.IsTrue( buttons.IsDown( button ) );
				Assert.IsFalse( buttons.IsUp( button ) );

				button = MouseButton.Right;
				Assert.IsFalse( buttons.IsClick( button ) );
				Assert.IsFalse( buttons.IsRelease( button ) );
				Assert.IsFalse( buttons.IsDown( button ) );
				Assert.IsTrue( buttons.IsUp( button ) );

				button = MouseButton.Middle;
				Assert.IsFalse( buttons.IsClick( button ) );
				Assert.IsFalse( buttons.IsRelease( button ) );
				Assert.IsFalse( buttons.IsDown( button ) );
				Assert.IsTrue( buttons.IsUp( button ) );
			}

			[Test]
			public void Right()
			{
				MouseButtons buttons = new MouseButtons()
				{
					Previous = MouseButton.None,
					Current = MouseButton.Right
				};

				var r = MouseButton.Right & MouseButton.Right;
				var b = r != 0;
				var i = (int)r;

				var button = MouseButton.Left;
				Assert.IsFalse( buttons.IsClick( button ) );
				Assert.IsFalse( buttons.IsRelease( button ) );
				Assert.IsFalse( buttons.IsDown( button ) );
				Assert.IsTrue( buttons.IsUp( button ) );

				button = MouseButton.Right;
				Assert.IsTrue( buttons.IsClick( button ) );
				Assert.IsFalse( buttons.IsRelease( button ) );
				Assert.IsTrue( buttons.IsDown( button ) );
				Assert.IsFalse( buttons.IsUp( button ) );

				button = MouseButton.Middle;
				Assert.IsFalse( buttons.IsClick( button ) );
				Assert.IsFalse( buttons.IsRelease( button ) );
				Assert.IsFalse( buttons.IsDown( button ) );
				Assert.IsTrue( buttons.IsUp( button ) );
			}

			[Test]
			public void Middle()
			{
				MouseButtons buttons = new MouseButtons()
				{
					Previous = MouseButton.None,
					Current = MouseButton.Middle
				};

				var button = MouseButton.Left;
				Assert.IsFalse( buttons.IsClick( button ) );
				Assert.IsFalse( buttons.IsRelease( button ) );
				Assert.IsFalse( buttons.IsDown( button ) );
				Assert.IsTrue( buttons.IsUp( button ) );

				button = MouseButton.Right;
				Assert.IsFalse( buttons.IsClick( button ) );
				Assert.IsFalse( buttons.IsRelease( button ) );
				Assert.IsFalse( buttons.IsDown( button ) );
				Assert.IsTrue( buttons.IsUp( button ) );

				button = MouseButton.Middle;
				Assert.IsTrue( buttons.IsClick( button ) );
				Assert.IsFalse( buttons.IsRelease( button ) );
				Assert.IsTrue( buttons.IsDown( button ) );
				Assert.IsFalse( buttons.IsUp( button ) );

			}
		}

	}
}
