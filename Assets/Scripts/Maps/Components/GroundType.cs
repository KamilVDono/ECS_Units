using Maps.Authoring;

using System;

using Unity.Collections;
using Unity.Entities;

using UnityEngine;

namespace Maps.Components
{
	[Serializable]
	public struct GroundType : ISharedComponentData, IEquatable<GroundType>
	{
		public BlobAssetReference<GroundTypeBlob> TileTypeBlob;

		public GroundType( TileTypeSO tileTypeSO )
		{
			CreateBlob( tileTypeSO );
		}

		public void CreateBlob( TileTypeSO TileTypeSO )
		{
			BlobBuilder blobBuilder = new BlobBuilder( Allocator.Persistent );
			ref GroundTypeBlob tileBlob = ref blobBuilder.ConstructRoot<GroundTypeBlob>();
			tileBlob.Material = TileTypeSO.Material;
			tileBlob.Mesh = TileTypeSO.Mesh;
			tileBlob.MoveCost = TileTypeSO.Cost;
			tileBlob.NoiseRange = TileTypeSO.Range;
			blobBuilder.AllocateString( ref tileBlob.Name, TileTypeSO.name );
			blobBuilder.AllocateString( ref tileBlob.Description, TileTypeSO.ToString() );
			TileTypeBlob = blobBuilder.CreateBlobAssetReference<GroundTypeBlob>( Allocator.Persistent );
			blobBuilder.Dispose();
		}

		public bool Equals( GroundType other ) => TileTypeBlob.Equals( other.TileTypeBlob );

		public override int GetHashCode() => TileTypeBlob.GetHashCode();
	}

	public struct GroundTypeBlob
	{
		public Material Material;
		public Mesh Mesh;
		public float MoveCost;
		public float NoiseRange;
		public BlobString Name;
		public BlobString Description;
	}
}