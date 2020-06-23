using FSM.Runtime.Nodes;

using System;

using UnityEngine;

namespace FSM.Runtime.Edges
{
	[Serializable]
	public class FSMTransition
	{
		[SerializeField] private string _editorName;
		[SerializeField] private FSMGraph _parent;
		[SerializeField] private int _from;
		[SerializeField] private int _to;

		public FSMGraph Parent => _parent;

		public string Name => $"{FromNode.Name}=>{ToNode.Name}";

		public int From => _from;
		public FSStateNode FromNode => Parent.Node( From );
		public int To => _to;
		public FSStateNode ToNode => Parent.Node( To );

		public FSMTransition( FSMGraph parent, int from, int to )
		{
			_from = from;
			_to = to;
			_parent = parent;
			_editorName = Name;
		}
	}
}