using FSM.Editor.Nodes;
using FSM.Runtime;

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
	}
}