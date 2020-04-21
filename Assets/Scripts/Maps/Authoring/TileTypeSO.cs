using Blobs.Interfaces;

using Maps.Components;

using System;

using Unity.Entities;

using UnityEngine;

namespace Maps.Authoring
{
	[CreateAssetMenu( menuName = "ScriptableObjects/Map/TileType" )]
	public class TileTypeSO : ScriptableObject, IBlobableSO<GroundTypeBlob>
	{
		public Color Color;
		public float Cost;

		[Range(0f, 1f)]
		public float Range;

		public bool AcceptResourceOre;

		public string ShaderName = "Map/Tile";

		#region Cache
		private string _toString = null;
		#endregion Cache

		#region Properties
		public BlobAssetReference<GroundTypeBlob> BlobReference { get; private set; }

		public Type BlobType => typeof( GroundTypeBlob );
		#endregion Properties

		public void SetupBlobReference()
		{
			SetupToString();
			Dispose();
			BlobReference = GroundTypeBlob.FromSO( this );
		}

		public override string ToString() => _toString;

		public void Dispose()
		{
			if ( BlobReference.IsCreated )
			{
				BlobReference.Dispose();
			}
		}

		private void SetupToString() => _toString = $"Tile {name}, cost {Cost};";
	}
}