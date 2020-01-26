using Input.Components;

using Maps.Components;

using Pathfinding.Components;

using Resources.Components;

using System.Text;

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
		private StringBuilder _descriptionBuilder = new StringBuilder( 50 );
		private EntityManager _entityManager;

		private void Awake()
		{
			_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			_tileUnderMouseQuery = _entityManager.CreateEntityQuery( ComponentType.ReadOnly<TileUnderMouse>() );
		}

		private void Update()
		{
			var infoComponents = _tileUnderMouseQuery.ToComponentDataArray<TileUnderMouse>( Unity.Collections.Allocator.TempJob );

			if ( infoComponents.Length == 1 )
			{
				if ( _lastTile != infoComponents[0].Tile )
				{
					_tileInfoText.text = ExtractDescription( infoComponents[0] );
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

		private void OnDestroy()
		{
			_tileUnderMouseQuery.Dispose();
		}

		private string ExtractDescription( TileUnderMouse tileUnderMouse )
		{
			var groundType = _entityManager.GetComponentData<GroundType>( tileUnderMouse.Tile );
			var resourceOre = _entityManager.GetComponentData<ResourceOre>( tileUnderMouse.Tile );

			var movementCost = _entityManager.GetComponentData<MovementCost>( tileUnderMouse.Tile );

			_descriptionBuilder.Clear();
			_descriptionBuilder.AppendLine( groundType.TileTypeBlob.Value.Description.ToString() );
			_descriptionBuilder.AppendLine( resourceOre.ToString() );
			if ( _entityManager.HasComponent<Stock>( tileUnderMouse.Tile ) )
			{
				var stock = _entityManager.GetComponentData<Stock>( tileUnderMouse.Tile );
				_descriptionBuilder.AppendLine( stock.ToString() );
			}
			_descriptionBuilder.AppendLine( movementCost.ToString() );

			return _descriptionBuilder.ToString();
		}
	}
}