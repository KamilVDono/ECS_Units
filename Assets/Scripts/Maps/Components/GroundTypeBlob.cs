using Blobs.Interfaces;

using Helpers.Extensions;
using Helpers.Types;

using Maps.Authoring;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Maps.Components
{
	public struct GroundTypeBlob : IBlobable<TileTypeSO>
	{
		public float MoveCost;
		public float NoiseRange;
		public float4 MainColor;
		public Boolean AcceptResourceOre;
		public BlobString ShaderName;
		public BlobString Name;
		public BlobString Description;

		public static BlobAssetReference<GroundTypeBlob> FromSO( TileTypeSO tileTypeSO )
		{
			BlobBuilder blobBuilder = new BlobBuilder( Allocator.Persistent );
			ref GroundTypeBlob tileBlob = ref blobBuilder.ConstructRoot<GroundTypeBlob>();
			tileBlob.MoveCost = tileTypeSO.Cost;
			tileBlob.NoiseRange = tileTypeSO.Range;
			tileBlob.MainColor = tileTypeSO.Color.AsFloat4();
			tileBlob.AcceptResourceOre = tileTypeSO.AcceptResourceOre;
			blobBuilder.AllocateString( ref tileBlob.Name, tileTypeSO.name );
			blobBuilder.AllocateString( ref tileBlob.Description, tileTypeSO.ToString() );
			blobBuilder.AllocateString( ref tileBlob.ShaderName, tileTypeSO.ShaderName );
			var blobReference = blobBuilder.CreateBlobAssetReference<GroundTypeBlob>( Allocator.Persistent );
			blobBuilder.Dispose();
			return blobReference;
		}
	}
}