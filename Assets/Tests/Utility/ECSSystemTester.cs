using NUnit.Framework;

using System.Reflection;

using Unity.Entities;

namespace Tests.Utility
{
	public abstract class ECSSystemTester<T> where T : ComponentSystemBase
	{
		protected World _previousWorld;
		protected World _currentWorld;
		protected EntityManager _entityManager;

		protected virtual bool _createAllSystems { get; } = false;
		protected T TargetSystem => _currentWorld.GetOrCreateSystem<T>();

		[SetUp]
		public virtual void SetUp()
		{
			_previousWorld = World.DefaultGameObjectInjectionWorld;
			_currentWorld = World.DefaultGameObjectInjectionWorld = new World( "Test World" );

			_entityManager = _currentWorld.EntityManager;

			if ( _createAllSystems )
			{
				// Right now not working :(
				//var allSystems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default, requireExecuteAlways: false);
				//allSystems.Add( typeof( ConstantDeltaTimeSystem ) ); //Need to be added with UpdateWorldTimeSystem at the same time.
				//DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups( w, allSystems );
			}
			else
			{
				_currentWorld.CreateSystem<ConstTimeMockSystem>();
				_currentWorld.CreateSystem<T>();
			}
		}

		[TearDown]
		public virtual void TearDown()
		{
			_currentWorld.Dispose();
			_currentWorld = null;

			World.DefaultGameObjectInjectionWorld = _previousWorld;
			_previousWorld = null;
		}

		protected virtual void Update()
		{
			if ( TargetSystem != null )
			{
				// Unfortunately, this do not work correctly
				//_currentWorld.Update();
				foreach ( var system in _currentWorld.Systems )
				{
					system.Update();
				}
				_entityManager.CompleteAllJobs();
			}
			else
			{
				throw new NUnit.Framework.InconclusiveException( "Can not localize system in world" );
			}
		}

		protected void Inject( object value, string fieldName )
		{
			var fieldInfo = TargetSystem.GetType().GetField( fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField );
			fieldInfo.SetValue( TargetSystem, value );
		}
	}
}