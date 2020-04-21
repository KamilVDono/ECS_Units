using FSM.Runtime.Edges;

using UnityEditor.Experimental.GraphView;

namespace FSM.Editor.Edges
{
	public class FSMEdgeView : Edge
	{
		private FSMTransition _transition = null;
		public FSMTransition Transition => _transition;

		public FSMEdgeView() : base()
		{
		}

		public FSMEdgeView( FSMTransition transition ) : base() => Initialize( transition );

		public void Initialize( FSMTransition transition ) => _transition = transition;
	}
}