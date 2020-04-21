using FSM.Runtime.Nodes;

using UnityEngine;

namespace FSM.Runtime.Edges
{
	public class FSMTransition : ScriptableObject
	{
		[SerializeField] private StateNode _from;
		[SerializeField] private StateNode _to;
		public StateNode From => _from;
		public StateNode To => _to;

		public void Init( FSMNode from, FSMNode to )
		{
			_from = from as StateNode;
			_to = to as StateNode;
			name = $"{_from.name}->{_to.name}";
		}
	}
}