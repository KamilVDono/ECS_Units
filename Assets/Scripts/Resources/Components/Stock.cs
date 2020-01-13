using Unity.Entities;

namespace Resources.Components
{
    public struct Stock : IComponentData
    {
        public int MaxSize;
        public BlobAssetReference<ResourceTypeBlob> Type;
        public int Count;
    }
}
