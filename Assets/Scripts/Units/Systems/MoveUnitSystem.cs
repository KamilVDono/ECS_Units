using Maps.Components;

using Pathfinding.Components;

using Units.Components.Stats;
using Units.Components.Tags;

using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Units.Systems
{
	public class MoveUnitSystem : JobComponentSystem
	{
		protected override void OnCreate() => RequireSingletonForUpdate<MapSettings>();

		protected override JobHandle OnUpdate( JobHandle inputDeps )
		{
			var mapSettings = GetSingleton<MapSettings>();

			var edgeSize = mapSettings.MapEdgeSize;

			// move job
			// TODO: check impossible way
			var deltaTime = Time.DeltaTime;
			var tiles = mapSettings.Tiles;
			var movementCosts = GetComponentDataFromEntity<MovementCost>( true );
			var moveHandleJob = Entities
				.WithAll<UnitTag, MovingTag>()
				.WithReadOnly(movementCosts)
				.WithReadOnly(tiles)
				.WithNativeDisableContainerSafetyRestriction( tiles )
				.ForEach( ( Entity e, int entityInQueryIndex, ref DynamicBuffer<Waypoint> waypoints, ref Translation translation, ref MapIndex mapIndex, in MovementSpeed movementSpeed ) =>
				{
					// out of waypoints
					if(waypoints.Length < 1)
					{
						return;
					}
					// check arrived (at waypoint and/or whole path)
					var lastPoint = waypoints[ waypoints.Length - 1 ];
					var position2D = new float2( translation.Value.x, translation.Value.z );

					var movementCost = movementCosts[tiles[mapIndex.Index1D]];
					var stepSize = movementSpeed.Speed * deltaTime;
					if(movementCost.Cost < 0.001f)
					{
						stepSize /= 0.001f;
					}
					else
					{
						stepSize /= movementCost.Cost;
					}

					// we reach waypoint if we are close as single step distance
					if ( math.distancesq( lastPoint.Position, position2D ) <= (stepSize * stepSize) )
					{
						waypoints.RemoveAt( waypoints.Length - 1 );
					}

					// out of waypoints, set location
					if(waypoints.Length < 1)
					{
						translation.Value = new float3(lastPoint.Position.x, translation.Value.y, lastPoint.Position.y);
						mapIndex = MapIndex.FromWorldPosition( translation.Value, edgeSize );
						return;
					}

					// calc new position and map index
					lastPoint = waypoints[ waypoints.Length - 1 ];
					var direction = math.normalize(lastPoint.Position - position2D);
					var translated2D = direction * stepSize + position2D;
					translation.Value = new float3(translated2D.x, translation.Value.y, translated2D.y);
					mapIndex = MapIndex.FromWorldPosition( translation.Value, edgeSize );
				} ).Schedule( inputDeps );

			return moveHandleJob;
		}
	}
}