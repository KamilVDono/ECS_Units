using Blobs.Interfaces;

using Maps.Components;

using System;

using Unity.Entities;

using UnityEngine;

namespace Maps.Authoring
{
	[CreateAssetMenu( menuName = "ScriptbleObjects/Map/TileType" )]
	public class TileTypeSO : ScriptableObject, IBlobableSO<GroundTypeBlob>
	{
		public Color32 Color;
		public float Cost;

		[Range(0f, 1f)]
		public float Range;

		public bool AcceptResourceOre;

		#region Cache
		private Material _material;

		private string _toString = null;
		#endregion Cache

		#region Properties
		public Material Material => _material != null ? _material : CreateMaterial();

		public BlobAssetReference<GroundTypeBlob> BlobReference { get; private set; }

		public Type BlobType => typeof( GroundTypeBlob );
		#endregion Properties

		public void SetupBlobReference()
		{
			SetupToString();
			Dispose();
			BlobReference = GroundTypeBlob.FromSO( this );
		}

		public override string ToString()
		{
			return _toString;
		}

		public void Dispose()
		{
			if ( BlobReference.IsCreated )
			{
				BlobReference.Dispose();
			}
		}

		private void SetupToString()
		{
			_toString = $"Tile {name}, cost {Cost};";
		}

		private Material CreateMaterial()
		{
			_material = new Material( Shader.Find( "Map/Tile" ) );
			_material.SetColor( "_MainColor", Color );
			_material.enableInstancing = true;
			return _material;
		}
	}
}