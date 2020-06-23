using FSM.Editor.Edges;
using FSM.Editor.Nodes;
using FSM.Runtime;
using FSM.Runtime.Edges;
using FSM.Runtime.Nodes;

using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;

namespace FSM.Editor
{
	public class FSMGraphView : GraphView
	{
		public static readonly Vector2 DEFAULT_NODE_SIZE = new Vector2(200, 100);
		private FSMGraph _stateMachineGraph;

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
			List<FSStateNode> connections = null;
			if ( startAnchor.direction == Direction.Input )
			{
				connections = startAnchor.connections.OfType<FSMEdgeView>().Select( e => e.Transition.ToNode as FSStateNode ).ToList();
			}
			else
			{
				connections = startAnchor.connections.OfType<FSMEdgeView>().Select( e => e.Transition.FromNode as FSStateNode ).ToList();
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
				evt.menu.AppendAction( "Add state", CreateNode );
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

			_stateMachineGraph = stateMachineGraph;

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

				Port outputPort = viewFrom.outputContainer.Query<Port>().First();
				Port inputPort = viewTo.inputContainer.Query<Port>().First();

				var edge = new FSMEdgeView(transition)
				{
					input = inputPort,
					output = outputPort,
				};

				outputPort.Connect( edge );
				inputPort.Connect( edge );

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

		public FSMTransition CreateTransition( FSStateNode from, FSStateNode to )
		{
			var transition = _stateMachineGraph.CreateTransition( from, to );
			EditorUtility.SetDirty( _stateMachineGraph );
			return transition;
		}

		private void DeleteEdge( FSMEdgeView edge )
		{
			edge.input.Disconnect( edge );
			edge.output.Disconnect( edge );

			edge.output = null;
			edge.input = null;

			RemoveElement( edge );
			_stateMachineGraph.RemoveTransition( edge.Transition );
		}

		#endregion Edge operations

		#region Node operations

		private void CreateNode( DropdownMenuAction dropdownMenuAction )
		{
			var newNode = _stateMachineGraph.CreateNode( MousePositionFromEvent(dropdownMenuAction.eventInfo), DEFAULT_NODE_SIZE );
			AddElement( new FSMNodeView( newNode ) );
			EditorUtility.SetDirty( _stateMachineGraph );
		}

		private void DeleteNode( FSMNodeView node )
		{
			var transitionViews = ports.ToList().SelectMany( p => p.connections ).OfType<FSMEdgeView>().Distinct();

			var transitionsToDelete = _stateMachineGraph.Connections( node.StateNode );

			foreach ( var toDelete in transitionViews.Where( tv => transitionsToDelete.Contains( tv.Transition ) ).ToArray() )
			{
				DeleteEdge( toDelete );
			}

			RemoveElement( node );
			_stateMachineGraph.RemoveNode( node.StateNode );
		}

		#endregion Node operations

		#region Utils

		private Vector2 MousePositionFromEvent( DropdownMenuEventInfo eventInfo )
		{
			var mousePosition = eventInfo.mousePosition;
			var graphMousePosition = contentViewContainer.WorldToLocal(mousePosition);
			return graphMousePosition;
		}

		private FSMNodeView ViewFor( int guid ) => nodes.ToList().OfType<FSMNodeView>().FirstOrDefault( v => v.StateNode.GUID == guid );

		#endregion Utils
	}
}