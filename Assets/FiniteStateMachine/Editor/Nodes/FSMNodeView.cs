using FSM.Runtime.Nodes;

using System.Linq;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;

namespace FSM.Editor.Nodes
{
	public class FSMNodeView : Node
	{
		private readonly Label _titleLable;
		public FSMNode StateNode { get; private set; }

		public FSMNodeView( FSMNode stateNode )
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
				EditorUtility.SetDirty( StateNode );
				titleContainer.Remove( textField );
				_titleLable.visible = true;
			} );
			titleContainer.Insert( 0, textField );
			_titleLable.visible = false;
			textField.Focus();
			textField.SelectAll();
		}

		public void Dump()
		{
			StateNode.Position = GetPosition();
			StateNode.name = title;
			EditorUtility.SetDirty( StateNode );
		}

		private void Load()
		{
			title = StateNode.name;
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
			addRow.Add( new Button() { text = "+" } );
			container.Add( addRow );
			container.Add( Divider() );
			container.Add( new Label( "Idle tag" ) );
			container.Add( new Label( "Idle tag 2" ) );

			var label = new Label("Click me");
			label.style.width = 200;
			label.style.height = 100;

			label.RegisterCallback<MouseUpEvent>( HandleRightClick );
			label.RegisterCallback<DragUpdatedEvent>( OnDragUpdatedEvent );
			label.RegisterCallback<DragPerformEvent>( OnDragPerformEvent );

			container.Add( label );

			extensionContainer.Add( container );
		}

		private void OnDragUpdatedEvent( DragUpdatedEvent e )
		{
			if ( DragAndDrop.objectReferences.Any( o => o is ScriptableObject ) )
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
			if ( DragAndDrop.objectReferences.Any( o => o is ScriptableObject ) )
			{
				DragAndDrop.AcceptDrag();
			}
			else
			{
				Debug.Log( "None so" );
			}
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
			public static void Show(
				Vector2 displayPosition )
			{
				var window = CreateInstance<CustomMenuWindow>();
				window.position = new Rect( displayPosition, new Vector2( 100, 200 ) );
				window.ShowPopup();
				window.Focus();
			}

			private void OnLostFocus() => Close();
		}
	}
}