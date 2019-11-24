#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityInputActions
{
#if UNITY_EDITOR

	[InitializeOnLoad]
#endif
	public class InitializationUnityInput
	{
#if UNITY_EDITOR

		static InitializationUnityInput()
		{
			Initialize();
		}

#endif

		[RuntimeInitializeOnLoadMethod]
		private static void Initialize() => InputSystem.RegisterBindingComposite<ConditionalComposite2D>();
	}
}