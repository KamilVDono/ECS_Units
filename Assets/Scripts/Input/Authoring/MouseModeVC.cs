using Input.Components;

using System;
using System.Linq;

using TMPro;

using Unity.Entities;

using UnityEngine;
using UnityEngine.UI;

using SF = UnityEngine.SerializeField;

namespace Input.Authoring
{
	public class MouseModeVC : MonoBehaviour
	{
		[SF] private TextMeshProUGUI _currentModedText = null;
		[SF] private Button _changeModeButton = null;

		private EntityManager _entityManager;
		private EntityQuery _mouseModeQuery;
		private int _maxModeValue;

		private int _clickCounter;
		private MouseMode _lastMode;

		private void Awake()
		{
			_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			_mouseModeQuery = _entityManager.CreateEntityQuery( ComponentType.ReadOnly<CurrentMouseMode>() );
			_maxModeValue = Enum.GetValues( typeof( MouseMode ) ).Cast<int>().Max() + 1;
			CreateMouseMode();

			_changeModeButton.onClick.RemoveAllListeners();
			_changeModeButton.onClick.AddListener( IncreaseClicks );
		}

		private void Update()
		{
			var mode = _mouseModeQuery.GetSingleton<CurrentMouseMode>().Mode;
			mode = (MouseMode)( ( ( (int)mode ) + _clickCounter ) % _maxModeValue );

			_clickCounter = 0;

			if ( mode != _lastMode )
			{
				_mouseModeQuery.SetSingleton<CurrentMouseMode>( new CurrentMouseMode { Mode = mode } );
				UpdateTextTo( mode );
				_lastMode = mode;
			}
		}

		private void CreateMouseMode()
		{
			var mode = MouseMode.Building;
			var entity = _entityManager.CreateEntity( typeof( CurrentMouseMode ) );
			_entityManager.SetComponentData( entity, new CurrentMouseMode { Mode = mode } );
			UpdateTextTo( mode );
		}

		private void UpdateTextTo( MouseMode mouseMode ) => _currentModedText.text = $"Mouse mode: {mouseMode}";

		private void IncreaseClicks() => _clickCounter++;
	}
}