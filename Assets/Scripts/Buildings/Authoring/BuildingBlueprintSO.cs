using Blobs.Interfaces;

using Buildings.Components;

using Resources.Authoring;

using System;

using Unity.Entities;

using UnityEngine;

namespace Buildings.Authoring
{
	[CreateAssetMenu( menuName = "ScriptbleObjects/Buildings/Blueprint" )]
	public class BuildingBlueprintSO : ScriptableObject, IBlobableSO<BuildingBlueprintBlob>
	{
		#region Data
		public ResourceQuantityPair[] RequiredResources;
		public int RequiredWork;
		#endregion Data

		#region IBlobableSO
		public BlobAssetReference<BuildingBlueprintBlob> BlobReference { get; private set; }

		public Type BlobType => typeof( BuildingBlueprintBlob );

		public void Dispose()
		{
			if ( BlobReference.IsCreated )
			{
				BlobReference.Dispose();
			}
		}

		public void SetupBlobReference()
		{
			Dispose();
			BlobReference = BuildingBlueprintBlob.FromSO( this );
		}
		#endregion IBlobableSO

		[System.Serializable]
		public class ResourceQuantityPair
		{
			public ResourceTypeSO ResourceType;
			public int Quantity;
		}
	}
}