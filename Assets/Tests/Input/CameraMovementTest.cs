using Input.Authoring;

using NUnit.Framework;

using System;

using UnityEngine;

namespace Tests.Input
{
	public class CameraMovementTest
	{
		public class CalculateBounds
		{
			[Test]
			public void CalculateBounds_ZeroTile()
			{
				var mapSize = 0;
				var cameraSize = 0.5f;
				var yPosition = 1f;
				var aspectRatio = 1f;

				Assert.Throws( typeof( ArgumentException ), () => CameraMovement.CalculateBounds( mapSize, cameraSize, yPosition, aspectRatio ) );
			}

			[Test]
			public void CalculateBounds_NegativeTile()
			{
				var mapSize = -1;
				var cameraSize = 0.5f;
				var yPosition = 1f;
				var aspectRatio = 1f;

				Assert.Throws( typeof( ArgumentException ), () => CameraMovement.CalculateBounds( mapSize, cameraSize, yPosition, aspectRatio ) );
			}

			[Test]
			public void CalculateBounds_OneTile()
			{
				var mapSize = 1;
				var cameraSize = 0.5f;
				var yPosition = 1f;
				var aspectRatio = 1f;

				Assert.DoesNotThrow( () => CameraMovement.CalculateBounds( mapSize, cameraSize, yPosition, aspectRatio ) );
			}

			[Test]
			public void CalculateBounds_OneTile_Portail()
			{
				var mapSize = 1;
				var cameraSize = 0.5f;
				var yPosition = 1f;
				var aspectRatio = 0.999f;

				Assert.Throws( typeof( ArgumentException ), () => CameraMovement.CalculateBounds( mapSize, cameraSize, yPosition, aspectRatio ) );
			}

			[Test]
			public void CalculateBounds_TenTile_Wide()
			{
				var mapSize = 10;
				var cameraSize = 0.5f;
				var yPosition = 1f;
				var aspectRatio = 1.77778f;

				Assert.DoesNotThrow( () => CameraMovement.CalculateBounds( mapSize, cameraSize, yPosition, aspectRatio ) );
			}

			[Test]
			public void CalculateBounds_OneTile_Square()
			{
				var mapSize = 1;
				var cameraSize = 0.5f;
				var yPosition = 1f;
				var aspectRatio = 1f;

				Assert.DoesNotThrow( () => CameraMovement.CalculateBounds( mapSize, cameraSize, yPosition, aspectRatio ) );
			}

			[Test]
			public void CalculateBounds_OneTile_Square_ZeroSize()
			{
				var mapSize = 1;
				var cameraSize = 0f;
				var yPosition = 1f;
				var aspectRatio = 1f;

				Assert.Throws( typeof( ArgumentException ), () => CameraMovement.CalculateBounds( mapSize, cameraSize, yPosition, aspectRatio ) );
			}

			[Test]
			public void CalculateBounds_OneTile_Square_NegativeSize()
			{
				var mapSize = 1;
				var cameraSize = -1f;
				var yPosition = 1f;
				var aspectRatio = 1f;

				Assert.Throws( typeof( ArgumentException ), () => CameraMovement.CalculateBounds( mapSize, cameraSize, yPosition, aspectRatio ) );
			}

			[Test]
			public void CalculateBounds_OneTile_Square_HalfSize()
			{
				var mapSize = 1;
				var cameraSize = 0.5f;
				var yPosition = 1f;
				var aspectRatio = 1f;

				var bounds = CameraMovement.CalculateBounds(mapSize, cameraSize, yPosition, aspectRatio);
				var expected = new Bounds(new Vector3(0, yPosition, 0), Vector3.zero);

				Assert.AreEqual( expected, bounds );
			}

			[Test]
			public void CalculateBounds_TenTile_Wide_OneSize()
			{
				var mapSize = 10;
				var cameraSize = 1f;
				var yPosition = 1f;
				var aspectRatio = 1.5f;

				var bounds = CameraMovement.CalculateBounds(mapSize, cameraSize, yPosition, aspectRatio);
				var expected = new Bounds(new Vector3(4.5f, yPosition, 4.5f), new Vector3(3.5f, 0, 4f) * 2);

				Assert.AreEqual( expected, bounds );
			}
		}

		public class CheckBounds
		{
			[Test]
			public void CheckBounds_OutsideLeft()
			{
				var bounds = new Bounds(new Vector3(5, 0, 5), new Vector3(1,0,1) * 2);
				var position = new Vector3(-1, 0, 5);

				var excepted = new Vector3(4, 0, 5);
				var actual = CameraMovement.CheckBounds(bounds, position);

				Assert.AreEqual( excepted, actual );
			}
		}
	}
}