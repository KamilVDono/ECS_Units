﻿using Blobs.Interfaces;

using Helpers.Extensions;
using Helpers.Types;

using Resources.Authoring;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Resources.Components
{
	public struct ResourceTypeBlob : IBlobable<ResourceTypeSO>
	{
		public BlobString Name;
		public float MovementCost;
		public float4 Color;
		public float WorkRequired;
		public int PiecesPerWork;
		public Boolean Stackable;
		public float UnitSize;

		public static BlobAssetReference<ResourceTypeBlob> FromSO( ResourceTypeSO resourceTypeSO )
		{
			BlobBuilder blobBuilder = new BlobBuilder( Allocator.Persistent );
			ref ResourceTypeBlob resourceTypeBlob = ref blobBuilder.ConstructRoot<ResourceTypeBlob>();
			blobBuilder.AllocateString( ref resourceTypeBlob.Name, resourceTypeSO.name );
			resourceTypeBlob.MovementCost = resourceTypeSO.MovementCost;
			resourceTypeBlob.Color = resourceTypeSO.Color.AsFloat4();
			resourceTypeBlob.WorkRequired = resourceTypeSO.WorkRequired;
			resourceTypeBlob.PiecesPerWork = resourceTypeSO.PiecesPerWork;
			resourceTypeBlob.Stackable = resourceTypeSO.Stackable;
			resourceTypeBlob.UnitSize = resourceTypeSO.UnitSize;
			var blobReference = blobBuilder.CreateBlobAssetReference<ResourceTypeBlob>( Allocator.Persistent );
			blobBuilder.Dispose();
			return blobReference;
		}
	}
}