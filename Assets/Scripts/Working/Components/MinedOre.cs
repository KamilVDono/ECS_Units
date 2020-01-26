using Resources.Components;

using Unity.Entities;

namespace Working.Components
{
	public struct MinedOre : IComponentData
	{
		public BlobAssetReference<ResourceTypeBlob> Type;
		public int MinedCount;
	}
}
