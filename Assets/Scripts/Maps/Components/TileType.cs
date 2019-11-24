using Maps.Authoring;

using System;

using Unity.Entities;

namespace Maps.Components
{
	[Serializable]
	public struct TileType : ISharedComponentData, IEquatable<TileType>
	{
		public TileTypeSO TileTypeSO;

		public bool Equals( TileType other ) => ReferenceEquals( TileTypeSO, other.TileTypeSO );

		public override int GetHashCode() => TileTypeSO.GetHashCode();
	}
}