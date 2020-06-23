using FSM.Runtime.Edges;
using FSM.Runtime.Nodes;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace FSM.Runtime
{
	[Serializable]
	[CreateAssetMenu( menuName = "ScriptableObjects/FSM/FSMGraph" )]
	public class FSMGraph : ScriptableObject
	{
		[SerializeField] private List<FSStateNode> _nodes = new List<FSStateNode>();
		[SerializeField] private List<FSMTransition> _transitions = new List<FSMTransition>();

		#region Queries
		public IReadOnlyCollection<FSStateNode> Nodes => _nodes;
		public IReadOnlyCollection<FSMTransition> Transitions => _transitions;

		public FSStateNode Node( int nodedGuid ) => _nodes.FirstOrDefault( n => n.GUID == nodedGuid );

		public IEnumerable<FSMTransition> Connections( FSStateNode node ) => _transitions.Where( t => t.From == node || t.To == node );

		#endregion Queries

		#region Elements operations

		public FSMTransition CreateTransition( FSStateNode from, FSStateNode to )
		{
			var transition = new FSMTransition(this, from.GUID, to.GUID);
			_transitions.Add( transition );
			return transition;
		}

		public void RemoveTransition( FSMTransition transition ) => _transitions.Remove( transition );

		public FSStateNode CreateNode( Vector2 nodePosition, Vector2 nodeSize )
		{
			var node = new FSStateNode( $"{typeof( FSStateNode ).Name}_{_nodes.Count}" )
			{
				Position = new Rect( nodePosition, nodeSize ),
				Parent = this,
			};
			_nodes.Add( node );
			return node;
		}

		public void RemoveNode( FSStateNode node )
		{
			foreach ( var transition in Connections( node ).ToArray() )
			{
				RemoveTransition( transition );
			}
			_nodes.Remove( node );
		}

		#endregion Elements operations
	}
}