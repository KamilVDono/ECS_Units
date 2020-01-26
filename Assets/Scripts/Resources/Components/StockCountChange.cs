using Unity.Entities;

namespace Resources.Components
{
	public struct StockCountChange : IComponentData
	{
		public BlobAssetReference<ResourceTypeBlob> Type;
		public int Count;
	}
}