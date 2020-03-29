using Helpers.Types;

using Maps.Components;

using Pathfinding.Components;

using Resources.Components;

using StateMachine.Components;

using Units.Components;
using Units.Components.Stats;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

using Working.Components;

namespace Units.Systems
{
	[UpdateInGroup( typeof( InitializationSystemGroup ) )]
	public class UnitTagsSystem : SystemBase
	{
		private EndInitializationEntityCommandBufferSystem _cmdBufferSystem;
		private NativeArray<Neighbor> _neighbors;

		protected override void OnCreate()
		{
			RequireSingletonForUpdate<MapSettings>();
			_cmdBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
			_neighbors = Neighbor.FullNeighborhood( Allocator.Persistent );
		}

		protected override void OnUpdate()
		{
			#region Seek for mine
			var seekingTagCB = _cmdBufferSystem.CreateCommandBuffer().ToConcurrent();

			var seekingTagHandle = Entities
				.WithAll<UnitTag, IdleTag>()
				.WithNone<SeekingOresTag>()
				.ForEach( ( Entity e, int entityInQueryIndex ) =>
				{
					seekingTagCB.AddComponent<SeekingOresTag>( entityInQueryIndex, e );
					seekingTagCB.RemoveComponent<IdleTag>( entityInQueryIndex, e );
				} ).ScheduleParallel( Dependency );
			#endregion Seek for mine

			#region Moving tag
			var movingTagCB = _cmdBufferSystem.CreateCommandBuffer().ToConcurrent();
			var seeking = GetComponentDataFromEntity<SeekingOresTag>(true);
			var idle = GetComponentDataFromEntity<IdleTag>(true);
			var movingTagHandle = Entities
				.WithReadOnly(seeking)
				.WithReadOnly(idle)
				.WithAll<UnitTag, Waypoint>()
				.WithNone<MovingTag>()
				.ForEach( ( Entity e, int entityInQueryIndex ) =>
				{
					if(seeking.HasComponent(e))
					{
						movingTagCB.RemoveComponent<SeekingOresTag>(entityInQueryIndex, e);
					}

					if(idle.HasComponent(e))
					{
						movingTagCB.RemoveComponent<IdleTag>(entityInQueryIndex, e);
					}

					movingTagCB.AddComponent<MovingTag>(entityInQueryIndex, e);
				} ).ScheduleParallel( Dependency );
			#endregion Moving tag

			#region Mining
			var arrivedCB = _cmdBufferSystem.CreateCommandBuffer().ToConcurrent();
			var tiles = GetSingleton<MapSettings>().Tiles;
			var ores = GetComponentDataFromEntity<ResourceOre>(true);
			var miningWork = GetComponentDataFromEntity<MiningWork>(true);
			var arrivedHandle = Entities
				.WithReadOnly(tiles)
				.WithNativeDisableContainerSafetyRestriction(tiles)
				.WithReadOnly(ores)
				.WithAll<UnitTag, MovingTag>()
				.WithNone<Waypoint>()
				.ForEach( (Entity e, int entityInQueryIndex, in MapIndex mapIndex, in MiningSpeed miningSpeed) =>
				{
					arrivedCB.RemoveComponent<MovingTag>( entityInQueryIndex, e );

					if(ores[tiles[mapIndex.Index1D]].IsValid)
					{
						arrivedCB.AddComponent<MiningWork>(entityInQueryIndex, tiles[mapIndex.Index1D],
							new MiningWork{ Worker = e, ProgressPerSecond = miningSpeed.Speed } );
						arrivedCB.AddComponent<MiningTag>(entityInQueryIndex, e);
					}
					else
					{
						arrivedCB.AddComponent<IdleTag>(entityInQueryIndex, e);
					}
				} ).ScheduleParallel(Dependency);
			#endregion Mining

			Dependency = JobHandle.CombineDependencies( movingTagHandle, seekingTagHandle, arrivedHandle );
			_cmdBufferSystem.AddJobHandleForProducer( Dependency );
		}

		protected override void OnDestroy() => _neighbors.Dispose();
	}
}