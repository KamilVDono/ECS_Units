using System;

using Unity.Entities;

using UnityEngine;

namespace Input.Components
{
	public struct CameraData : ISharedComponentData, IEquatable<CameraData>
	{
		public Camera Camera;

		public bool Equals( CameraData other )
		{
			return ReferenceEquals( Camera, other.Camera );
		}

		public override int GetHashCode()
		{
			return Camera.GetHashCode();
		}
	}
}