using Unity.Entities;

namespace Resources.Components
{
	public struct Stock : IComponentData
	{
		public int Capacity;
		public BlobAssetReference<ResourceTypeBlob> Type;
		public int Count;

		public override string ToString()
		{
			return $"Stock: {Type.Value.Name.ToString()} {Count}/{Capacity}";
		}
	}
}