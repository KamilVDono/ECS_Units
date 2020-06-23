using System;

using UnityEngine;

namespace FSM.Runtime.Nodes
{
	[Serializable]
	public class FSStateNode : IEquatable<FSStateNode>
	{
		#region Serialization data
		[SerializeField] private string _name;
		[SerializeField] private int _GUID;
		[SerializeField] private Rect _position;
		[SerializeField] private FSMGraph _parent;
		#endregion Serialization data

		#region Properties
		public string Name { get => _name; set => _name = value; }
		public int GUID => _GUID;

		public FSMGraph Parent { get => _parent; set => _parent = value; }
		public Rect Position { get => _position; set => _position = value; }
		#endregion Properties

		#region Constructors

		public FSStateNode( string name )
		{
			_GUID = Guid.NewGuid().GetHashCode();
			_name = name;
		}

		#endregion Constructors

		#region Equality

		public static bool operator ==( FSStateNode left, FSStateNode right ) => left.Equals( right );

		public static bool operator ==( FSStateNode left, int right ) => left.GUID.Equals( right );

		public static bool operator ==( int left, FSStateNode right ) => left.Equals( right.GUID );

		public static bool operator !=( FSStateNode left, FSStateNode right ) => !( left == right );

		public static bool operator !=( FSStateNode left, int right ) => !( left == right );

		public static bool operator !=( int left, FSStateNode right ) => !( left == right );

		public override bool Equals( object obj ) => Equals( obj as FSStateNode );

		public bool Equals( FSStateNode other ) => !ReferenceEquals( other, null ) && GUID.Equals( other.GUID );

		public override int GetHashCode() => GUID;

		#endregion Equality
	}
}