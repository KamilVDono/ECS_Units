﻿using System;

using Unity.Entities;

namespace Maps.Components
{
	[Serializable]
	public struct GroundType : IComponentData, IEquatable<GroundType>
	{
		public BlobAssetReference<GroundTypeBlob> TileTypeBlob;

		public GroundType( BlobAssetReference<GroundTypeBlob> tileType ) => TileTypeBlob = tileType;

		public bool Equals( GroundType other ) => TileTypeBlob.Equals( other.TileTypeBlob );

		public override int GetHashCode() => TileTypeBlob.GetHashCode();
	}
}