using System;

using UnityEngine;

using SF = UnityEngine.SerializeField;

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
		[SF] private Material _baseMaterial;
		private Material _material;

		private string _toString = null;

		public Material Material => _material != null ? _material : CreateMaterial();

		public bool Equals( TileTypeSO other ) => ReferenceEquals( this, other );

		public override string ToString() => _toString;

		private void OnEnable() =>
			_toString = $"Tile with cost: {Cost}, color: {Color}, mesh: {Mesh.name} and material {_baseMaterial.name} ({_baseMaterial.shader.name})";

		private Material CreateMaterial()
		{
			_material = new Material( _baseMaterial );
			_material.SetColor( "_BaseColor", Color );
			return _material;
		}
	}
}