using Input.Components;

using Maps.Components;

using Pathfinding.Components;

using Resources.Components;

using System.Text;

using TMPro;

using Units.Components.Stats;
using Units.Components.Tags;

using Unity.Entities;

using UnityEngine;

using SF = UnityEngine.SerializeField;

namespace Input.Authoring
{
	// TODO: Change query to just component singleton 
	public class TileInfoVC : MonoBehaviour
	{
		[SF] private TextMeshProUGUI _tileInfoText = null;
		[SF] private TextMeshProUGUI _unitInfoText = null;
		private EntityQuery _tileUnderMouseQuery;
		private StringBuilder _descriptionBuilder = new StringBuilder( 50 );
		private EntityManager _entityManager;

		private void Awake()
		{
			_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			_tileUnderMouseQuery = _entityManager.CreateEntityQuery( ComponentType.ReadOnly<TileUnderMouse>(), ComponentType.ReadOnly<UnitUnderMouse>() );
		}

		private void Update()
		{
			var tileInfoComponents = _tileUnderMouseQuery.ToComponentDataArray<TileUnderMouse>( Unity.Collections.Allocator.TempJob );
			var unitInfoComponents = _tileUnderMouseQuery.ToComponentDataArray<UnitUnderMouse>( Unity.Collections.Allocator.TempJob );

			// One mouse data and tile under mouse if valid tile
			if ( tileInfoComponents.Length == 1 && _entityManager.HasComponent<GroundType>( tileInfoComponents[0].Tile ) )
			{
				_tileInfoText.text = ExtractTileDescription( tileInfoComponents[0] );
			}
			else
			{
				_tileInfoText.text = "";
				Debug.LogWarning( "There is no or more than 1 TileUnderMouse" );
			}

			if ( unitInfoComponents.Length == 1 && _entityManager.HasComponent<UnitTag>( unitInfoComponents[0].Unit ) )
			{
				_unitInfoText.text = ExtractUnitDescription( unitInfoComponents[0] );
			}
			else
			{
				_unitInfoText.text = "";
			}

			tileInfoComponents.Dispose();
			unitInfoComponents.Dispose();
		}

		private void OnDestroy() => _tileUnderMouseQuery.Dispose();

		private string ExtractTileDescription( TileUnderMouse tileUnderMouse )
		{
			var groundType = _entityManager.GetComponentData<GroundType>( tileUnderMouse.Tile );
			var resourceOre = _entityManager.GetComponentData<ResourceOre>( tileUnderMouse.Tile );
			var movementCost = _entityManager.GetComponentData<MovementCost>( tileUnderMouse.Tile );

			_descriptionBuilder.Clear();
			_descriptionBuilder.AppendLine( groundType.TileTypeBlob.Value.Description.ToString() );
			if ( resourceOre.IsValid )
			{
				_descriptionBuilder.AppendLine( resourceOre.ToString() );
			}
			if ( _entityManager.HasComponent<Stock>( tileUnderMouse.Tile ) )
			{
				var stock = _entityManager.GetComponentData<Stock>( tileUnderMouse.Tile );
				_descriptionBuilder.AppendLine( stock.ToString() );
			}
			_descriptionBuilder.AppendLine( movementCost.ToString() );

			return _descriptionBuilder.ToString();
		}

		private string ExtractUnitDescription( UnitUnderMouse unitUnderMouse )
		{
			var movementSpeed = _entityManager.GetComponentData<MovementSpeed>( unitUnderMouse.Unit) ;
			var miningSpeed = _entityManager.GetComponentData<MiningSpeed>( unitUnderMouse.Unit );

			_descriptionBuilder.Clear();
			if ( _entityManager.HasComponent<MiningTag>( unitUnderMouse.Unit ) )
			{
				_descriptionBuilder.AppendLine( "Unit is <color=#feffd6>MINING</color>: " );
			}
			else if ( _entityManager.HasComponent<MovingTag>( unitUnderMouse.Unit ) )
			{
				_descriptionBuilder.AppendLine( "Unit is <color=#feffd6>MOVING</color>: " );
			}
			else if ( _entityManager.HasComponent<SeekingOresTag>( unitUnderMouse.Unit ) )
			{
				_descriptionBuilder.AppendLine( "Unit is <color=#feffd6>SEEKING FOR ORES TO MINING</color>: " );
			}
			else
			{
				_descriptionBuilder.AppendLine( "Unit: " );
			}

			_descriptionBuilder.AppendLine( $" * Movement speed: {movementSpeed.Speed:0.00}" );
			_descriptionBuilder.AppendLine( $" * Mining speed: {miningSpeed.Speed:0.00}" );
			return _descriptionBuilder.ToString();
		}
	}
}