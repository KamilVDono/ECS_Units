using FSM.Runtime.Nodes;

using System;
using System.Linq;
using System.Text.RegularExpressions;

using Unity.Entities;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.IMGUI.Controls;

using UnityEngine;
using UnityEngine.UIElements;

namespace FSM.Editor.Nodes
{
	public class FSMNodeView : Node
	{
		private static readonly Regex _namespaceRegex = new Regex(@"(namespace) (.+)");

		private readonly Label _titleLable;
		public FSStateNode StateNode { get; private set; }

		public FSMNodeView( FSStateNode stateNode )
		{
			capabilities &= ~Capabilities.Deletable;
			capabilities |= Capabilities.Renamable;
			//capabilities |= Capabilities.Resizable;

			_titleLable = this.Q<Label>( "title-label" );

			StateNode = stateNode;

			Load();

			GeneratePorts();
			GenerateContent();

			RefreshExpandedState();
			RefreshPorts();
		}

		public void Rename()
		{
			var textField = new TextField
			{
				isDelayed = true
			};
			textField.SetValueWithoutNotify( title );
			textField.RegisterValueChangedCallback( ( change ) =>
			{
				title = change.newValue;
				EditorUtility.SetDirty( StateNode.Parent );
				titleContainer.Remove( textField );
				_titleLable.visible = true;
			} );
			titleContainer.Insert( 0, textField );
			_titleLable.visible = false;
			textField.Focus();
			textField.SelectAll();
			Dump();
		}

		public void Dump()
		{
			StateNode.Position = GetPosition();
			StateNode.Name = title;
		}

		private void Load()
		{
			title = StateNode.Name;
			SetPosition( StateNode.Position );
		}

		private void GenerateContent()
		{
			var container = new VisualElement
			{
				name = "Content"
			};

			var addRow = RowContainer();

			addRow.Add( new Label( "Required data:" ) );
			var button = new Button() { text = "+" };
			button.style.flexGrow = 1f;
			addRow.Add( button );
			container.Add( addRow );

			addRow.RegisterCallback<MouseUpEvent>( HandleRightClick );
			addRow.RegisterCallback<DragUpdatedEvent>( OnDragUpdatedEvent );
			addRow.RegisterCallback<DragPerformEvent>( OnDragPerformEvent );

			extensionContainer.Add( container );
		}

		private void OnDragUpdatedEvent( DragUpdatedEvent e )
		{
			if ( DragAndDrop.objectReferences.Any( o => o is ScriptableObject || o is MonoScript ) )
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Link;
			}
			else
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
			}
		}

		private void OnDragPerformEvent( DragPerformEvent e )
		{
			var scripts = DragAndDrop.objectReferences.OfType<MonoScript>();
			foreach ( var script in scripts )
			{
				var scriptClass = script.GetClass();

				var match = _namespaceRegex.Match( script.text );

				var namespaceName = (match.Success ? match.Groups[2].Value : "").Trim();

				var type = typeof(IComponentData);
				var types = AppDomain.CurrentDomain.GetAssemblies()
						.SelectMany(s => s.GetTypes())
						.Where(p => type.IsAssignableFrom(p)).ToList();

				var dropedType = types.FirstOrDefault( t => t.Name == script.name && t.Namespace == namespaceName );

				if ( dropedType != null )
				{
					Debug.Log( dropedType.FullName );
				}
				else
				{
					Debug.Log( "Can not find" );
				}
			}
			DragAndDrop.AcceptDrag();
		}

		private void HandleRightClick( MouseUpEvent evt )
		{
			if ( evt.button == (int)MouseButton.RightMouse )
			{
				CustomMenuWindow.Show( ( evt.target as VisualElement ).LocalToWorld( evt.localMousePosition ) );
				evt.StopPropagation();
			}

			return;
		}

		private VisualElement Divider()
		{
			var divider = new VisualElement
			{
				name = "divider",
			};
			divider.AddToClassList( "horizontal" );
			return divider;
		}

		private VisualElement RowContainer()
		{
			var container = new VisualElement
			{
				name = "row_container"
			};
			container.style.flexDirection = FlexDirection.Row;
			return container;
		}

		private void GeneratePorts()
		{
			inputContainer.Add( FSMNodePort.Create( EdgeConnectorListener.Instance, Direction.Input, "Input" ) );
			outputContainer.Add( FSMNodePort.Create( EdgeConnectorListener.Instance, Direction.Output, "Output" ) );
		}

		private class CustomMenuWindow : EditorWindow
		{
			private SearchField _searchField;

			public static void Show(
				Vector2 displayPosition )
			{
				var window = CreateInstance<CustomMenuWindow>();
				window.position = new Rect( displayPosition, new Vector2( 100, 200 ) );
				window.ShowPopup();
				window.Focus();
			}

			private void OnEnable() => _searchField = new SearchField();

			private void OnGUI() => _searchField.OnGUI( new Rect( 0, 0, 100, 25 ), "Search" );

			private void OnLostFocus() => Close();
		}
	}
}