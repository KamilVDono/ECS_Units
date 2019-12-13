using System;

using UnityEngine;

namespace Maps.Authoring
{
	[CreateAssetMenu( menuName = "ScriptbleObjects/Map/TileType" )]
	public class TileTypeSO : ScriptableObject, IEquatable<TileTypeSO>
	{
		public Color32 Color;
		public float Cost;
		public Mesh Mesh;
		[Range(0f, 1f)]
		public float Range;
		private Material _material;

		private string _toString = null;

		public Material Material => _material != null ? _material : CreateMaterial();

		public bool Equals( TileTypeSO other ) => ReferenceEquals( this, other );

		public override string ToString() => _toString;

		public void SetupToString() =>
			_toString = $"Tile with cost: {Cost}, color: {Color}, mesh: {Mesh?.name}";

		private Material CreateMaterial()
		{
			_material = new Material( Shader.Find( "Map/Tile" ) );
			_material.SetColor( "_MainColor", Color );
			return _material;
		}
	}
}