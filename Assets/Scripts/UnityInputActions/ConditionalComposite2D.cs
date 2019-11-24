using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

namespace UnityInputActions
{
	public class ConditionalComposite2D : InputBindingComposite<Vector2>
	{
		[InputControl(layout = "Button")]
		public int ConditionalButton;

		[InputControl(layout = "Button")]
		public int X;

		[InputControl(layout = "Button")]
		public int Y;

		public override Vector2 ReadValue( ref InputBindingCompositeContext context )
		{
			var condition = context.ReadValue<float>( ConditionalButton );
			return new Vector2( -context.ReadValue<float>( X ) / 5f, -context.ReadValue<float>( Y ) / 5f ) * condition;
		}
	}
}