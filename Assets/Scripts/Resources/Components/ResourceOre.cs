using Unity.Entities;

namespace Resources.Components
{
    public struct ResourceOre : IComponentData
    {
        public BlobAssetReference<ResourceTypeBlob> Type;
        public int Capacity;
        public int Count;

        public override string ToString() => $"Ore: {Type.Value.Name} {Count}/{Capacity}";
    }
}
