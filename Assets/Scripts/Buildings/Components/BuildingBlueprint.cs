using Helpers.Types;

using Unity.Entities;

namespace Buildings.Components
{
	public struct BuildingBlueprint : IComponentData
	{
		public BlobAssetReference<BuildingBlueprintBlob> Blueprint;
		public Boolean MeetResources;
		public Boolean MeetWork;
		public Boolean MeetRequirements => MeetResources && MeetWork;

		public BuildingBlueprint( BlobAssetReference<BuildingBlueprintBlob> blueprint ) : this()
		{
			Blueprint = blueprint;
			MeetResources = false;
			MeetWork = false;
		}
	}
}