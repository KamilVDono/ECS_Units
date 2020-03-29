using System;

using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

namespace Input.Components
{
	public struct CameraData : ISharedComponentData, IEquatable<CameraData>
	{
		public Camera Camera;

		public float2 ScreenToWorldPoint2D( Vector2 screenPosition )
		{
			var worldPosition = Camera.ScreenToWorldPoint( screenPosition );
			return new float2( worldPosition.x, worldPosition.z );
		}

		public bool Equals( CameraData other ) => ReferenceEquals( Camera, other.Camera );

		public override int GetHashCode() => Camera.GetHashCode();
	}
}