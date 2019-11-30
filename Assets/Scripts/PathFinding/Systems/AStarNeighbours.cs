using Helpers;

using Maps.Components;

using PathFinding.Components;
using PathFinding.Helpers;

using Unity.Collections;
using Unity.Entities;

namespace PathFinding.Systems
{
	[UpdateBefore( typeof( AStar ) )]
	public class AStarNeighbours : ComponentSystem
	{
		protected override void OnUpdate() =>
			Entities.WithNone<MapSettingsNeighborsState>().ForEach(
				( Entity e, ref MapSettings mapSettings ) =>
						{
							MapSettingsNeighborsState neighboursState = new MapSettingsNeighborsState();
							if ( mapSettings.CanMoveDiagonally )
							{
								neighboursState.Neighbours = new BlitableArray<Neighbor>( 8, Allocator.Persistent )
								{
									[0] = Neighbor.UPPER_LEFT,
									[1] = Neighbor.UPPER,
									[2] = Neighbor.UPPER_RIGHT,
									[3] = Neighbor.LEFT,
									[4] = Neighbor.RIGHT,
									[5] = Neighbor.BOTTOM_LEFT,
									[6] = Neighbor.BOTTOM,
									[7] = Neighbor.BOTTOM_RIGHT,
								};
							}
							else
							{
								neighboursState.Neighbours = new BlitableArray<Neighbor>( 4, Allocator.Persistent )
								{
									[0] = Neighbor.UPPER,
									[1] = Neighbor.LEFT,
									[2] = Neighbor.RIGHT,
									[3] = Neighbor.BOTTOM,
								};
							}
							PostUpdateCommands.AddComponent( e, neighboursState );
						} );
	}
}