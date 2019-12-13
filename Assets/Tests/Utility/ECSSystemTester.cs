using NUnit.Framework;

using Unity.Entities;

namespace Tests
{
	public abstract class ECSSystemTester<T> where T : ComponentSystem
	{
		protected World _previousWorld;
		protected World _currentWorld;
		protected EntityManager _entityManager;

		public T TargetSystem => World.Active.GetOrCreateSystem<T>();

		[SetUp]
		public virtual void SetUp()
		{
			_previousWorld = World.Active;
			_currentWorld = World.Active = new World( "Test World" );

			_entityManager = _currentWorld.EntityManager;
		}

		[TearDown]
		public virtual void TearDown()
		{
			if ( _entityManager != null )
			{
				_currentWorld.Dispose();
				_currentWorld = null;

				World.Active = _previousWorld;
				_previousWorld = null;
				_entityManager = null;
			}
		}

		protected virtual void Update()
		{
			if ( TargetSystem != null )
			{
				foreach ( var system in _currentWorld.Systems )
				{
					system.Update();
				}
			}
			else
			{
				throw new NUnit.Framework.InconclusiveException( "Can not localize system in world" );
			}
		}
	}
}
