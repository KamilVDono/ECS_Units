using Resources.Components;

using Unity.Entities;

namespace Buildings.Components
{
	public struct BuildStockElement : IBufferElementData
	{
		public BlobAssetReference<ResourceTypeBlob> ResourceType;
		public int Count;
	}
}