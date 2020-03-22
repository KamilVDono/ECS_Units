using NUnit.Framework;

using Tests.Categories;
using Tests.Utility;

using Unity.Entities;

using Working.Components;
using Working.Systems;

namespace Tests.Working
{
	public class WorkProgressionTest : ECSSystemTester<WorkProgressionSystem>
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			var ese = _currentWorld.CreateSystem<EndSimulationEntityCommandBufferSystem>();
			Inject( ese, "_removeCmdBufferSystem" );
		}

		[Test]
		[ECSTest]
		public void MinigWork_OneUpdate()
		{
			var workEntity = _entityManager.CreateEntity( typeof( WorkProgress ), typeof( MiningWork ) );
			_entityManager.SetComponentData( workEntity, new WorkProgress() );
			_entityManager.SetComponentData( workEntity, new MiningWork { ProgressPerSecond = 1f } );

			Update();

			Assert.AreEqual( ConstTimeMockSystem.DELTA_TIME, TargetSystem.Time.DeltaTime );
			Assert.AreEqual( TargetSystem.Time.DeltaTime, _entityManager.GetComponentData<WorkProgress>( workEntity ).Progress );
		}

		[Test]
		[ECSTest]
		public void MinigWork_OneProgress()
		{
			var workEntity = _entityManager.CreateEntity( typeof( WorkProgress ), typeof( MiningWork ) );
			_entityManager.SetComponentData( workEntity, new WorkProgress() );
			_entityManager.SetComponentData( workEntity, new MiningWork { ProgressPerSecond = 1f } );

			for ( int i = 0; i < 60; i++ )
			{
				Update();
			}

			Assert.AreEqual( ConstTimeMockSystem.DELTA_TIME, TargetSystem.Time.DeltaTime );
			Assert.AreEqual( TargetSystem.Time.DeltaTime * 60, _entityManager.GetComponentData<WorkProgress>( workEntity ).Progress, 0.001f );
		}
	}
}