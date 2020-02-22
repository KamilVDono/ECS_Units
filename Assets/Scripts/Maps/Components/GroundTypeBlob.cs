using Blobs.Interfaces;

using Helpers.Types;

using Maps.Authoring;

using Unity.Collections;
using Unity.Entities;

using UnityEngine;

namespace Maps.Components
{
	public struct GroundTypeBlob : IBlobable<TileTypeSO>
	{
		public Material Material;
		public float MoveCost;
		public float NoiseRange;
		public Boolean AcceptResourceOre;
		public BlobString Name;
		public BlobString Description;

		public static BlobAssetReference<GroundTypeBlob> FromSO( TileTypeSO tileTypeSO )
		{
			BlobBuilder blobBuilder = new BlobBuilder( Allocator.Persistent );
			ref GroundTypeBlob tileBlob = ref blobBuilder.ConstructRoot<GroundTypeBlob>();
			tileBlob.Material = tileTypeSO.Material;
			tileBlob.MoveCost = tileTypeSO.Cost;
			tileBlob.NoiseRange = tileTypeSO.Range;
			tileBlob.AcceptResourceOre = tileTypeSO.AcceptResourceOre;
			blobBuilder.AllocateString( ref tileBlob.Name, tileTypeSO.name );
			blobBuilder.AllocateString( ref tileBlob.Description, tileTypeSO.ToString() );
			var blobReference = blobBuilder.CreateBlobAssetReference<GroundTypeBlob>( Allocator.Persistent );
			blobBuilder.Dispose();
			return blobReference;
		}
	}
}