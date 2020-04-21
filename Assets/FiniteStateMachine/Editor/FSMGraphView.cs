using FSM.Editor.Edges;
using FSM.Editor.Nodes;
using FSM.Runtime;
using FSM.Runtime.Edges;
using FSM.Runtime.Nodes;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;

namespace FSM.Editor
{
	public class FSMGraphView : GraphView
	{
		public static readonly Vector2 DEFAULT_NODE_SIZE = new Vector2(200, 100);

		public FSMGraphView()
		{
			// Styles
			styleSheets.Add( UnityEngine.Resources.Load<StyleSheet>( "FSMGraphView" ) );

			// Setup build-in interaction
			SetupZoom( ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale );
			this.AddManipulator( new ContentDragger() );
			this.AddManipulator( new SelectionDragger() );
			this.AddManipulator( new RectangleSelector() );

			// Add grid
			var grid = new GridBackground();
			Insert( 0, grid );
			grid.StretchToParentSize();

			deleteSelection += DeleteSelected;
		}

		public override List<Port> GetCompatiblePorts( Port startAnchor, NodeAdapter nodeAdapter )
		{
			var compatibleAnchors = new List<Port>();
			List<FSMNode> connections = null;
			if ( startAnchor.direction == Direction.Input )
			{
				connections = startAnchor.connections.OfType<FSMEdgeView>().Select( e => e.Transition.To as FSMNode ).ToList();
			}
			else
			{
				connections = startAnchor.connections.OfType<FSMEdgeView>().Select( e => e.Transition.From as FSMNode ).ToList();
			}

			foreach ( var candidateAnchor in ports.ToList() )
			{
				if ( startAnchor.node == candidateAnchor.node )
				{
					continue;
				}

				if ( startAnchor.direction == candidateAnchor.direction )
				{
					continue;
				}
				if ( connections.Any( c => c == ( candidateAnchor.node as FSMNodeView )?.StateNode ) )
				{
					continue;
				}

				compatibleAnchors.Add( candidateAnchor );
			}

			return compatibleAnchors;
		}

		public override void BuildContextualMenu( ContextualMenuPopulateEvent evt )
		{
			if ( evt.target is GraphView )
			{
				evt.menu.AppendAction( "Add state", AddNewNode );
			}
			else if ( evt.target is FSMNodeView node )
			{
				evt.menu.AppendAction( "Rename", ( _ ) => node.Rename() );
				evt.menu.AppendAction( "Delete", ( _ ) => DeleteNode( node ) );
			}
			else if ( evt.target is FSMEdgeView edge )
			{
				evt.menu.AppendAction( "Delete", ( _ ) => DeleteEdge( edge ) );
			}
			else
			{
				base.BuildContextualMenu( evt );
			}
		}

		public void LoadGraph( FSMGraph stateMachineGraph )
		{
			// Clear
			edges.ToList().ForEach( e => RemoveElement( e ) );
			nodes.ToList().ForEach( n => RemoveElement( n ) );

			// Load nodes
			foreach ( var node in stateMachineGraph.Nodes )
			{
				var nodeView = new FSMNodeView(node);
				AddElement( nodeView );
			}

			// Restore connections
			foreach ( var transition in stateMachineGraph.Transitions )
			{
				var viewFrom = ViewFor( transition.From );
				var viewTo = ViewFor( transition.To );

				Port fromPort = viewFrom.inputContainer.Query<Port>().First();
				Port toPort = viewTo.outputContainer.Query<Port>().First();

				var edge = new FSMEdgeView(transition)
				{
					input = fromPort,
					output = toPort,
				};

				fromPort.Connect( edge );
				toPort.Connect( edge );

				AddElement( edge );
			}
		}

		public override EventPropagation DeleteSelection()
		{
			DeleteSelected( "delete", AskUser.DontAskUser );
			return EventPropagation.Stop;
		}

		private void DeleteSelected( string operationName, AskUser askUser )
		{
			selection.OfType<FSMNodeView>().ToList().ForEach( n => DeleteNode( n ) );
			selection.OfType<FSMEdgeView>().ToList().ForEach( n => DeleteEdge( n ) );
		}

		#region Edge operations

		public event Func<FSMNode, FSMNode, FSMTransition> NewTransitionRequest;

		public event Action<FSMTransition> DeleteTransitionRequest;

		public FSMTransition CreateTransition( FSMNode from, FSMNode to ) => NewTransitionRequest?.Invoke( from, to );

		private void DeleteEdge( FSMEdgeView edge )
		{
			edge.input.Disconnect( edge );
			edge.output.Disconnect( edge );

			edge.output = null;
			edge.input = null;

			RemoveElement( edge );
			DeleteTransitionRequest?.Invoke( edge.Transition );
		}

		#endregion Edge operations

		#region Node operations

		public event Func<Type, Vector2, FSMNode> NewNodeRequest;

		public event Action<FSMNode> DeleteNodeRequest;

		private void DeleteNode( FSMNodeView node )
		{
			RemoveElement( node );
			DeleteNodeRequest?.Invoke( node.StateNode );
		}

		private void AddNewNode( DropdownMenuAction dropdownMenuAction )
		{
			if ( NewNodeRequest == null )
			{
				return;
			}

			var newNode = NewNodeRequest(typeof(StateNode), MousePositionFromEvent(dropdownMenuAction.eventInfo) );
			AddElement( new FSMNodeView( newNode ) );
		}

		#endregion Node operations

		#region Utils

		private Vector2 MousePositionFromEvent( DropdownMenuEventInfo eventInfo )
		{
			var mousePosition = eventInfo.mousePosition;
			var graphMousePosition = contentViewContainer.WorldToLocal(mousePosition);
			return graphMousePosition;
		}

		private FSMNodeView ViewFor( FSMNode node ) => nodes.ToList().OfType<FSMNodeView>().FirstOrDefault( v => v.StateNode == node );

		#endregion Utils
	}
}