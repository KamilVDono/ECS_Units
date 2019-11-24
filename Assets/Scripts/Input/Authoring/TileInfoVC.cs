using Input.Components;

using Maps.Authoring;
using Maps.Components;

using TMPro;

using Unity.Entities;

using UnityEngine;

using SF = UnityEngine.SerializeField;

namespace Input.Authoring
{
	public class TileInfoVC : MonoBehaviour
	{
		[SF] private TextMeshProUGUI _tileInfoText;
		private EntityQuery _tileUnderMouseQuery;
		private Entity _lastTile;

		private static TileTypeSO ExtractTileSO( TileUnderMouse tileUnderMouse ) =>
			World.Active.EntityManager.GetSharedComponentData<TileType>( tileUnderMouse.Tile ).TileTypeSO;

		private void Awake() => _tileUnderMouseQuery = World.Active.EntityManager.CreateEntityQuery( ComponentType.ReadOnly<TileUnderMouse>() );

		private void Update()
		{
			var infoComponents = _tileUnderMouseQuery.ToComponentDataArray<TileUnderMouse>( Unity.Collections.Allocator.TempJob );

			if ( infoComponents.Length == 1 )
			{
				if ( _lastTile != infoComponents[0].Tile )
				{
					_tileInfoText.text = ExtractTileSO( infoComponents[0] ).ToString();
					_lastTile = infoComponents[0].Tile;
				}
			}
			else
			{
				_tileInfoText.transform.parent.gameObject.SetActive( false );
				Debug.LogWarning( "There is no or more than 1 TileUnderMouse" );
			}

			infoComponents.Dispose();
		}

		private void OnDestroy() => _tileUnderMouseQuery.Dispose();
	}
}