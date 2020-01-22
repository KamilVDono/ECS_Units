using Unity.Core;
using Unity.Entities;

namespace Tests.Utility
{
	[UpdateInGroup( typeof( InitializationSystemGroup ) )]
	// Can not reference right now
	//[UpdateAfter( typeof( UpdateWorldTimeSystem ) )]
	[DisableAutoCreation]
	public class ConstTimeMockSystem : ComponentSystem
	{
		public const float DELTA_TIME = 1f / 60f;
		private float _elapsedTime;

		protected override void OnCreate()
		{
			//Only works if Unity's time system is there already, this would
			//try to replace it. You should not add this system before Unity's.
			//var timeSystem = World.GetExistingSystem<UpdateWorldTimeSystem>();
			//if ( timeSystem != null )
			//{
			//    timeSystem.Enabled = false;
			//}
		}

		protected override void OnUpdate()
		{
			_elapsedTime += DELTA_TIME;
			World.SetTime( new TimeData(
				elapsedTime: _elapsedTime,
				deltaTime: DELTA_TIME
			) );
		}
	}
}