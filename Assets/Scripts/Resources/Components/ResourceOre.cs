using Helpers;

using Unity.Entities;

namespace Resources.Components
{
	public struct ResourceOre : IComponentData
	{
		public static readonly ResourceOre EMPTY_ORE = new ResourceOre() { Capacity = 0, Count = 0, Type = BlobAssetReference<ResourceTypeBlob>.Null };

		public BlobAssetReference<ResourceTypeBlob> Type;
		public int Capacity;
		public int Count;

		public Boolean Empty => Count == 0;
		public Boolean None => Type.IsCreated == false;
		public Boolean IsValid => Empty == false && None == false;

		public override string ToString()
		{
			if ( None == false )
			{
				return $"Ore: {Type.Value.Name.ToString()} {Count}/{Capacity}, cost {Type.Value.MovementCost}";
			}
			else
			{
				return "Ore: NONE";
			}
		}
	}
}