using FSM.Runtime.Edges;
using FSM.Runtime.Nodes;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace FSM.Runtime
{
	[Serializable]
	[CreateAssetMenu( menuName = "ScriptableObjects/FSM/FSMGraph" )]
	public class FSMGraph : ScriptableObject
	{
		[SerializeField] private List<FSMNode> _nodes = new List<FSMNode>();
		[SerializeField] private List<FSMTransition> _transitions = new List<FSMTransition>();
		public IReadOnlyCollection<FSMNode> Nodes => _nodes;
		public IReadOnlyCollection<FSMTransition> Transitions => _transitions;

		public void AddNode( FSMNode node ) => _nodes.Add( node );

		public void RemoveNode( FSMNode node ) => _nodes.Remove( node );

		public void AddTransition( FSMTransition transition ) => _transitions.Add( transition );

		public void RemoveTransition( FSMTransition transition ) => _transitions.Remove( transition );
	}
}