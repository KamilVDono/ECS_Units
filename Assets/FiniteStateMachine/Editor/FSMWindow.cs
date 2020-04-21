using FSM.Editor.Nodes;
using FSM.Runtime;
using FSM.Runtime.Edges;
using FSM.Runtime.Nodes;

using System;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace FSM.Editor
{
	public class FSMWindow : EditorWindow
	{
		private FSMGraphView _graphView;
		public FSMGraph StateMachineGraph { get; private set; }

		#region Initialization

		private void OnEnable()
		{
			if ( StateMachineGraph != null )
			{
				Initialize( StateMachineGraph );
			}
		}

		private void Initialize( FSMGraph stateMachineGraph )
		{
			StateMachineGraph = stateMachineGraph;
			titleContent.text = stateMachineGraph.name;

			CreateGraphView();
			CreateToolbar();
		}

		private void CreateGraphView()
		{
			_graphView = new FSMGraphView()
			{
				name = StateMachineGraph.name
			};
			_graphView.LoadGraph( StateMachineGraph );
			_graphView.NewNodeRequest += CreateNewNode;
			_graphView.DeleteNodeRequest += DeleteNode;
			_graphView.NewTransitionRequest += CreateNewTransition;
			_graphView.DeleteTransitionRequest += DeteleTransition;

			_graphView.StretchToParentSize();
			rootVisualElement.Add( _graphView );

			_graphView.RegisterCallback<KeyDownEvent>( OnKeyDown );
		}

		private void OnKeyDown( KeyDownEvent evt )
		{
			if ( evt.keyCode == KeyCode.S && evt.modifiers.HasFlag( EventModifiers.Control ) )
			{
				SaveAsset();
			}
			else if ( evt.keyCode == KeyCode.Delete )
			{
				_graphView.DeleteSelection();
			}
			else if ( evt.keyCode == KeyCode.F2 )
			{
				var nodeViews = _graphView.selection.OfType<FSMNodeView>().ToList();
				if ( nodeViews.Count == 1 )
				{
					nodeViews[0].Rename();
				}
			}
		}

		#endregion Initialization

		#region Node operations

		private FSMNode CreateNewNode( Type nodeType, Vector2 mousePosition )
		{
			FSMNode node = null;
			if ( typeof( FSMNode ).IsAssignableFrom( nodeType ) )
			{
				node = ScriptableObject.CreateInstance( nodeType ) as FSMNode;
				node.name = nodeType.Name;

				node.Position = new Rect( mousePosition, FSMGraphView.DEFAULT_NODE_SIZE );
			}

			if ( node != null )
			{
				StateMachineGraph.AddNode( node );
				AssetDatabase.AddObjectToAsset( node, StateMachineGraph );
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			return node;
		}

		private void DeleteNode( FSMNode nodeToDelete )
		{
			StateMachineGraph.RemoveNode( nodeToDelete );
			AssetDatabase.RemoveObjectFromAsset( nodeToDelete );
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		#endregion Node operations

		#region Transition operations

		private FSMTransition CreateNewTransition( FSMNode from, FSMNode to )
		{
			FSMTransition transition = ScriptableObject.CreateInstance<FSMTransition>();
			transition.Init( from, to );
			StateMachineGraph.AddTransition( transition );
			AssetDatabase.AddObjectToAsset( transition, StateMachineGraph );
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			return transition;
		}

		private void DeteleTransition( FSMTransition transition )
		{
			StateMachineGraph.RemoveTransition( transition );
			AssetDatabase.RemoveObjectFromAsset( transition );
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		#endregion Transition operations

		#region Toolbar

		private void CreateToolbar()
		{
			var toolbar = new Toolbar();

			var saveButton = new Button( SaveAsset )
			{
				text = "Save Asset"
			};
			toolbar.Add( saveButton );

			var pingButton = new Button( PingAsset )
			{
				text = "Show In Project"
			};
			toolbar.Add( pingButton );

			rootVisualElement.Add( toolbar );
		}

		private void SaveAsset()
		{
			_graphView.nodes.ToList().ForEach( n => ( n as FSMNodeView )?.Dump() );
			EditorUtility.SetDirty( StateMachineGraph );
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private void PingAsset() => EditorGUIUtility.PingObject( StateMachineGraph );

		#endregion Toolbar

		#region Opening

		[MenuItem( "Window/AI/StateMachine" )]
		public static void OpenWindow()
		{
			var currentGraph = Selection.activeObject as FSMGraph;
			var currentEditors = Resources.FindObjectsOfTypeAll<FSMWindow>();
			foreach ( var editor in currentEditors )
			{
				if ( editor.StateMachineGraph == currentGraph )
				{
					editor.Focus();
					return;
				}
			}

			FSMWindow window = CreateWindow<FSMWindow>( );

			window.Open( currentGraph );
		}

		[UnityEditor.Callbacks.OnOpenAsset( 1 )]
		public static bool OnOpenAsset( int instanceID, int line )
		{
			var asset = AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GetAssetPath( instanceID ) );
			if ( asset is FSMGraph graph )
			{
				OpenWindow();
				return true;
			}

			return false;
		}

		public void Open( FSMGraph stateMachineGraph )
		{
			if ( stateMachineGraph == null )
			{
				var path = EditorUtility.SaveFilePanel(
					"Save texture as PNG",
					Application.dataPath,
					"StateMachine.asset",
					"asset"
				);

				if ( path.Length == 0 )
				{
					this.Close();
					return;
				}

				// Create new StateMachineGraph
				var graphName = Path.GetFileNameWithoutExtension( path );
				stateMachineGraph = ScriptableObject.CreateInstance<FSMGraph>();
				stateMachineGraph.name = graphName;

				var databasePath = FileUtil.GetProjectRelativePath( path );

				AssetDatabase.CreateAsset( stateMachineGraph, databasePath );
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			Initialize( stateMachineGraph );
			Show();
		}

		#endregion Opening

		#region Cleanup

		private void OnDisable()
		{
			if ( _graphView != null )
			{
				_graphView.NewNodeRequest -= CreateNewNode;
				_graphView.DeleteNodeRequest -= DeleteNode;
				_graphView.NewTransitionRequest -= CreateNewTransition;
				_graphView.DeleteTransitionRequest -= DeteleTransition;
			}
		}

		#endregion Cleanup
	}
}