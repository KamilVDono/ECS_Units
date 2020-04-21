using Blobs.Interfaces;

using Resources.Components;

using System;

using Unity.Entities;

using UnityEngine;

namespace Resources.Authoring
{
	[CreateAssetMenu( menuName = "ScriptableObjects/Resources/ResourceType" )]
	public class ResourceTypeSO : ScriptableObject, IBlobableSO<ResourceTypeBlob>
	{
		public float MovementCost;
		public Color32 Color;
		public float WorkRequired;
		public int PiecesPerWork;
		public bool Stackable;
		public float UnitSize;

		public BlobAssetReference<ResourceTypeBlob> BlobReference { get; private set; }

		public Type BlobType => typeof( ResourceTypeBlob );

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
			BlobReference = ResourceTypeBlob.FromSO( this );
		}
	}
}