using Input.Authoring;

using NUnit.Framework;

using Unity.Mathematics;

using UnityEngine;

namespace Tests.Input
{
	public class ScreenEdgeInputTest
	{
		private static readonly ScreenEdgeInput SQUARE_100_10 = new ScreenEdgeInput(new int2(100, 100), 0.1f);
		private static readonly ScreenEdgeInput SQUARE_100_50 = new ScreenEdgeInput(new int2(100, 100), 0.5f);

		private static readonly ScreenEdgeInput WIDE_200_100_10 = new ScreenEdgeInput(new int2(200, 100), 0.1f);
		private static readonly ScreenEdgeInput WIDE_200_100_25 = new ScreenEdgeInput(new int2(200, 100), 0.25f);

		public class DigitalInput
		{
			public class Square10
			{
				[Test]
				public void DigitalInput_Square10_Center()
				{
					var mouseInput = new Vector2(50, 50);

					var actual = SQUARE_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(0, 0);

					Assert.AreEqual( expected, actual );
				}

				#region Square 10 Left

				[Test]
				public void DigitalInput_Square10_LeftSmallEgde()
				{
					var mouseInput = new Vector2(10, 50);

					var actual = SQUARE_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(-1, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square10_LeftFullEgde()
				{
					var mouseInput = new Vector2(0, 50);

					var actual = SQUARE_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(-1, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square10_LeftNegative()
				{
					var mouseInput = new Vector2(-10, 50);

					var actual = SQUARE_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(-1, 0);

					Assert.AreEqual( expected, actual );
				}

				#endregion Square 10 Left

				#region Square 10 Right

				[Test]
				public void DigitalInput_Square10_RightSmallEgde()
				{
					var mouseInput = new Vector2(90, 50);

					var actual = SQUARE_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(1, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square10_RightFullEgde()
				{
					var mouseInput = new Vector2(100, 50);

					var actual = SQUARE_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(1, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square10_RightOver()
				{
					var mouseInput = new Vector2(110, 50);

					var actual = SQUARE_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(1, 0);

					Assert.AreEqual( expected, actual );
				}

				#endregion Square 10 Right

				#region Square 10 Up

				[Test]
				public void DigitalInput_Square10_UpSmallEgde()
				{
					var mouseInput = new Vector2(50, 90);

					var actual = SQUARE_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(0, 1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square10_UpFullEgde()
				{
					var mouseInput = new Vector2(50, 100);

					var actual = SQUARE_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(0, 1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square10_UpOver()
				{
					var mouseInput = new Vector2(50, 110);

					var actual = SQUARE_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(0, 1);

					Assert.AreEqual( expected, actual );
				}

				#endregion Square 10 Up

				#region Square 10 Down

				[Test]
				public void DigitalInput_Square10_DownSmallEgde()
				{
					var mouseInput = new Vector2(50, 10);

					var actual = SQUARE_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(0, -1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square10_DownFullEgde()
				{
					var mouseInput = new Vector2(50, 0);

					var actual = SQUARE_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(0, -1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square10_DownNegative()
				{
					var mouseInput = new Vector2(50, -10);

					var actual = SQUARE_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(0, -1);

					Assert.AreEqual( expected, actual );
				}

				#endregion Square 10 Down
			}

			public class Square50
			{
				[Test]
				public void DigitalInput_Square50_Center()
				{
					var mouseInput = new Vector2(50, 50);

					var actual = SQUARE_100_50.DigitalInput( mouseInput );
					var expected = new Vector2(-1, -1);

					Assert.AreEqual( expected, actual );
				}

				#region Square 50 Left

				[Test]
				public void DigitalInput_Square50_LeftSmallEgde()
				{
					var mouseInput = new Vector2(10, 50);

					var actual = SQUARE_100_50.DigitalInput( mouseInput );
					var expected = new Vector2(-1, -1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square50_LeftFullEgde()
				{
					var mouseInput = new Vector2(0, 50);

					var actual = SQUARE_100_50.DigitalInput( mouseInput );
					var expected = new Vector2(-1, -1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square50_LeftNegative()
				{
					var mouseInput = new Vector2(-10, 50);

					var actual = SQUARE_100_50.DigitalInput( mouseInput );
					var expected = new Vector2(-1, -1);

					Assert.AreEqual( expected, actual );
				}

				#endregion Square 50 Left

				#region Square 50 Right

				[Test]
				public void DigitalInput_Square50_RightSmallEgde()
				{
					var mouseInput = new Vector2(90, 50);

					var actual = SQUARE_100_50.DigitalInput( mouseInput );
					var expected = new Vector2(1, -1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square50_RightFullEgde()
				{
					var mouseInput = new Vector2(100, 50);

					var actual = SQUARE_100_50.DigitalInput( mouseInput );
					var expected = new Vector2(1, -1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square50_RightOver()
				{
					var mouseInput = new Vector2(110, 50);

					var actual = SQUARE_100_50.DigitalInput( mouseInput );
					var expected = new Vector2(1, -1);

					Assert.AreEqual( expected, actual );
				}

				#endregion Square 50 Right

				#region Square 50 Up

				[Test]
				public void DigitalInput_Square50_UpSmallEgde()
				{
					var mouseInput = new Vector2(50, 90);

					var actual = SQUARE_100_50.DigitalInput( mouseInput );
					var expected = new Vector2(-1, 1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square50_UpFullEgde()
				{
					var mouseInput = new Vector2(50, 100);

					var actual = SQUARE_100_50.DigitalInput( mouseInput );
					var expected = new Vector2(-1, 1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square50_UpOver()
				{
					var mouseInput = new Vector2(50, 110);

					var actual = SQUARE_100_50.DigitalInput( mouseInput );
					var expected = new Vector2(-1, 1);

					Assert.AreEqual( expected, actual );
				}

				#endregion Square 50 Up

				#region Square 50 Down

				[Test]
				public void DigitalInput_Square50_DownSmallEgde()
				{
					var mouseInput = new Vector2(50, 10);

					var actual = SQUARE_100_50.DigitalInput( mouseInput );
					var expected = new Vector2(-1, -1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square50_DownFullEgde()
				{
					var mouseInput = new Vector2(50, 0);

					var actual = SQUARE_100_50.DigitalInput( mouseInput );
					var expected = new Vector2(-1, -1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Square50_DownNegative()
				{
					var mouseInput = new Vector2(50, -10);

					var actual = SQUARE_100_50.DigitalInput( mouseInput );
					var expected = new Vector2(-1, -1);

					Assert.AreEqual( expected, actual );
				}

				#endregion Square 50 Down
			}

			public class Wide10
			{
				[Test]
				public void DigitalInput_Wide10_Center()
				{
					var mouseInput = new Vector2(50, 50);

					var actual = WIDE_200_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(0, 0);

					Assert.AreEqual( expected, actual );
				}

				#region Wide 10 Left

				[Test]
				public void DigitalInput_Wide10_LeftSmallEgde()
				{
					var mouseInput = new Vector2(10, 50);

					var actual = WIDE_200_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(-1, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Wide10_LeftFullEgde()
				{
					var mouseInput = new Vector2(0, 50);

					var actual = WIDE_200_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(-1, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Wide10_LeftNegative()
				{
					var mouseInput = new Vector2(-10, 50);

					var actual = WIDE_200_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(-1, 0);

					Assert.AreEqual( expected, actual );
				}

				#endregion Wide 10 Left

				#region Wide 10 Right

				[Test]
				public void DigitalInput_Wide10_RightSmallEgde()
				{
					var mouseInput = new Vector2(190, 50);

					var actual = WIDE_200_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(1, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Wide10_RightFullEgde()
				{
					var mouseInput = new Vector2(200, 50);

					var actual = WIDE_200_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(1, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Wide10_RightOver()
				{
					var mouseInput = new Vector2(210, 50);

					var actual = WIDE_200_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(1, 0);

					Assert.AreEqual( expected, actual );
				}

				#endregion Wide 10 Right

				#region Wide 10 Up

				[Test]
				public void DigitalInput_Wide10_UpSmallEgde()
				{
					var mouseInput = new Vector2(100, 190);

					var actual = WIDE_200_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(0, 1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Wide10_UpFullEgde()
				{
					var mouseInput = new Vector2(100, 200);

					var actual = WIDE_200_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(0, 1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Wide10_UpOver()
				{
					var mouseInput = new Vector2(100, 210);

					var actual = WIDE_200_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(0, 1);

					Assert.AreEqual( expected, actual );
				}

				#endregion Wide 10 Up

				#region Wide 10 Down

				[Test]
				public void DigitalInput_Wide10_DownSmallEgde()
				{
					var mouseInput = new Vector2(100, 10);

					var actual = WIDE_200_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(0, -1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Wide10_DownFullEgde()
				{
					var mouseInput = new Vector2(100, 0);

					var actual = WIDE_200_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(0, -1);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void DigitalInput_Wide10_DownNegative()
				{
					var mouseInput = new Vector2(100, -10);

					var actual = WIDE_200_100_10.DigitalInput( mouseInput );
					var expected = new Vector2(0, -1);

					Assert.AreEqual( expected, actual );
				}

				#endregion Wide 10 Down
			}
		}

		public class AnalogInput
		{
			public class Square10
			{
				[Test]
				public void AnalogInput_Square10_Center()
				{
					var mouseInput = new Vector2(50, 50);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(0, 0);

					Assert.AreEqual( expected, actual );
				}

				#region Square 10 Left

				[Test]
				public void AnalogInput_Square10_LeftSmallEgde()
				{
					var mouseInput = new Vector2(10, 50);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(0, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Square10_LeftHalfEgde()
				{
					var mouseInput = new Vector2(5, 50);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(-0.5f, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Square10_LeftFullEgde()
				{
					var mouseInput = new Vector2(0, 50);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(-1f, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Square10_LeftNegative()
				{
					var mouseInput = new Vector2(-10, 50);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(-1, 0);

					Assert.AreEqual( expected, actual );
				}

				#endregion Square 10 Left

				#region Square 10 Right

				[Test]
				public void AnalogInput_Square10_RightSmallEgde()
				{
					var mouseInput = new Vector2(90, 50);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(0, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Square10_RightHalfEgde()
				{
					var mouseInput = new Vector2(95, 50);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(0.5f, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Square10_RightFullEgde()
				{
					var mouseInput = new Vector2(100, 50);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(1f, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Square10_RightOver()
				{
					var mouseInput = new Vector2(110, 50);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(1f, 0);

					Assert.AreEqual( expected, actual );
				}

				#endregion Square 10 Right

				#region Square 10 Down

				[Test]
				public void AnalogInput_Square10_DownSmallEgde()
				{
					var mouseInput = new Vector2(50, 10);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(0, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Square10_DownHalfEgde()
				{
					var mouseInput = new Vector2(50, 5);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(0, -0.5f);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Square10_DownFullEgde()
				{
					var mouseInput = new Vector2(50, 0);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(0, -1f);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Square10_DownNegative()
				{
					var mouseInput = new Vector2(50, -10);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(0, -1f);

					Assert.AreEqual( expected, actual );
				}

				#endregion Square 10 Down

				#region Square 10 Up

				[Test]
				public void AnalogInput_Square10_UpSmallEgde()
				{
					var mouseInput = new Vector2(50, 90);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(0, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Square10_UpHalfEgde()
				{
					var mouseInput = new Vector2(50, 95);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(0, 0.5f);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Square10_UpFullEgde()
				{
					var mouseInput = new Vector2(50, 100);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(0, 1f);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Square10_UpOver()
				{
					var mouseInput = new Vector2(50, 110);

					var actual = SQUARE_100_10.AnalogInput( mouseInput );
					var expected = new Vector2(0, 1f);

					Assert.AreEqual( expected, actual );
				}

				#endregion Square 10 Up
			}

			public class Wide25
			{
				[Test]
				public void AnalogInput_Wide25_Center()
				{
					var mouseInput = new Vector2(100, 50);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(0, 0);

					Assert.AreEqual( expected, actual );
				}

				#region Wide 25 Left

				[Test]
				public void AnalogInput_Wide25_LeftSmallEgde()
				{
					var mouseInput = new Vector2(50, 50);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(0, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Wide25_LeftHalfEgde()
				{
					var mouseInput = new Vector2(25, 50);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(-0.5f, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Wide25_LeftFullEgde()
				{
					var mouseInput = new Vector2(0, 50);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(-1f, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Wide25_LeftNegative()
				{
					var mouseInput = new Vector2(-70, 50);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(-1, 0);

					Assert.AreEqual( expected, actual );
				}

				#endregion Wide 25 Left

				#region Wide 25 Right

				[Test]
				public void AnalogInput_Wide25_RightSmallEgde()
				{
					var mouseInput = new Vector2(150, 50);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(0, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Wide25_RightHalfEgde()
				{
					var mouseInput = new Vector2(175, 50);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(0.5f, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Wide25_RightFullEgde()
				{
					var mouseInput = new Vector2(200, 50);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(1f, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Wide25_RightOver()
				{
					var mouseInput = new Vector2(300, 50);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(1f, 0);

					Assert.AreEqual( expected, actual );
				}

				#endregion Wide 25 Right

				#region Wide 25 Down

				[Test]
				public void AnalogInput_Wide25_DownSmallEgde()
				{
					var mouseInput = new Vector2(100, 25);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(0, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Wide25_DownHalfEgde()
				{
					var mouseInput = new Vector2(100, 12.5f);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(0, -0.5f);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Wide25_DownFullEgde()
				{
					var mouseInput = new Vector2(100, 0);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(0, -1f);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Wide25_DownNegative()
				{
					var mouseInput = new Vector2(100, -50);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(0, -1f);

					Assert.AreEqual( expected, actual );
				}

				#endregion Wide 25 Down

				#region Wide 25 Up

				[Test]
				public void AnalogInput_Wide25_UpSmallEgde()
				{
					var mouseInput = new Vector2(100, 75);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(0, 0);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Wide25_UpHalfEgde()
				{
					var mouseInput = new Vector2(100, 87.5f);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(0, 0.5f);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Wide25_UpFullEgde()
				{
					var mouseInput = new Vector2(100, 100);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(0, 1f);

					Assert.AreEqual( expected, actual );
				}

				[Test]
				public void AnalogInput_Wide25_UpOver()
				{
					var mouseInput = new Vector2(100, 150);

					var actual = WIDE_200_100_25.AnalogInput( mouseInput );
					var expected = new Vector2(0, 1f);

					Assert.AreEqual( expected, actual );
				}

				#endregion Wide 25 Up
			}
		}
	}
}