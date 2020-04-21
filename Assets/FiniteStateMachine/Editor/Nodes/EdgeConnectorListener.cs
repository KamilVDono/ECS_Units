using FSM.Editor.Edges;
using FSM.Runtime.Edges;

using UnityEditor.Experimental.GraphView;

using UnityEngine;

namespace FSM.Editor.Nodes
{
	public class EdgeConnectorListener : IEdgeConnectorListener
	{
		private static EdgeConnectorListener _instance;

		public static EdgeConnectorListener Instance => _instance ?? ( _instance = new EdgeConnectorListener() );

		public void OnDropOutsidePort( Edge edge, Vector2 position )
		{
		}

		public void OnDrop( GraphView graphView, Edge edge )
		{
			edge.capabilities |= Capabilities.Selectable;

			// Copy from unity default connector
			if ( graphView is FSMGraphView fsmGrphView )
			{
				graphView.AddElement( edge );
				edge.input.Connect( edge );
				edge.output.Connect( edge );

				if ( edge is FSMEdgeView fsmEdge )
				{
					FSMTransition tansition = fsmGrphView.CreateTransition(
							( edge.input.node as FSMNodeView ).StateNode,
							( edge.output.node as FSMNodeView ).StateNode
							);
					fsmEdge.Initialize( tansition );
				}
			}
		}
	}
}