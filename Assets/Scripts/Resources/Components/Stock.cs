using Unity.Entities;
using Unity.Mathematics;

namespace Resources.Components
{
	public struct Stock : IComponentData
	{
		public int Capacity;
		public BlobAssetReference<ResourceTypeBlob> Type;
		public int Count;

		public int AvailableSpace => (int)math.floor( ( Capacity - (int)math.ceil( Type.Value.UnitSize * Count ) ) / Type.Value.UnitSize );

		public override string ToString()
		{
			return $"Stock: {Type.Value.Name.ToString()} {Count}/{Capacity}";
		}
	}
}