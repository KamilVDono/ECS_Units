using Blobs.Interfaces;

using Buildings.Authoring;

using Resources.Components;

using Unity.Collections;
using Unity.Entities;

namespace Buildings.Components
{
	public struct BuildingBlueprintBlob : IBlobable<BuildingBlueprintSO>
	{
		public BlobString Name;
		public BlobArray<ResourceQuantityPair> RequiredResources;
		public int RequiredWork;

		public static BlobAssetReference<BuildingBlueprintBlob> FromSO( BuildingBlueprintSO buildingBlueprintSO )
		{
			BlobBuilder blobBuilder = new BlobBuilder( Allocator.Persistent );
			ref BuildingBlueprintBlob buildingBlueprintBlob = ref blobBuilder.ConstructRoot<BuildingBlueprintBlob>();
			blobBuilder.AllocateString( ref buildingBlueprintBlob.Name, buildingBlueprintSO.name );
			var requiredResourcesBuilder = blobBuilder.Allocate( ref buildingBlueprintBlob.RequiredResources, buildingBlueprintSO.RequiredResources.Length );

			for ( var i = 0; i < buildingBlueprintSO.RequiredResources.Length; i++ )
			{
				var required = buildingBlueprintSO.RequiredResources[i];
				ResourceQuantityPair requiredResource = new ResourceQuantityPair
				{
					ResourceType = required.ResourceType.BlobReference,
					Quantity = required.Quantity,
				};
				requiredResourcesBuilder[i] = requiredResource;
			}

			buildingBlueprintBlob.RequiredWork = buildingBlueprintSO.RequiredWork;

			var blobReference = blobBuilder.CreateBlobAssetReference<BuildingBlueprintBlob>( Allocator.Persistent );
			blobBuilder.Dispose();
			return blobReference;
		}
	}

	public struct ResourceQuantityPair
	{
		public BlobAssetReference<ResourceTypeBlob> ResourceType;
		public int Quantity;
	}
}