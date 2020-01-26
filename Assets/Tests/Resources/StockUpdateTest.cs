using Blobs;
using Blobs.Interfaces;

using NUnit.Framework;

using Resources.Authoring;
using Resources.Components;
using Resources.Systems;

using Tests.Utility;

using Unity.Entities;

using UnityEngine;

namespace Tests.Resources
{
	// System requires:
	// Stock, StockCountChange
	// uses system EndSimulationEntityCommandBufferSystem

	public class StockUpdateTest : ECSSystemTester<StockUpdateSystem>
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			ResourceTypeSO coal = ScriptableObject.CreateInstance<ResourceTypeSO>();
			coal.MovementCost = 2;
			coal.WorkRequired = 1f;
			coal.PiecesPerWork = 2;

			BlobsMemory.FromSOs( new IBlobableSO[] { coal } );

			var ese = _currentWorld.CreateSystem<EndSimulationEntityCommandBufferSystem>();
			Inject( ese, "_removeCmdBufferSystem" );
		}

		[Test]
		public void SimpleUpdate_ExistingStock()
		{
			var resourceType = BlobsMemory.Instance.ReferencesOf<ResourceTypeBlob>()[0];

			var stockEntity = _entityManager.CreateEntity( typeof( Stock ), typeof( StockCountChange ) );
			_entityManager.SetComponentData( stockEntity, new Stock() { Capacity = 50, Count = 0, Type = resourceType } );
			_entityManager.SetComponentData( stockEntity, new StockCountChange() { Count = 1, Type = resourceType } );

			Update();

			Assert.AreEqual( 1, _entityManager.GetComponentData<Stock>( stockEntity ).Count );
			Assert.IsFalse( _entityManager.HasComponent<StockCountChange>( stockEntity ) );
		}

		[Test]
		public void OverCapacityUpdate_ExistingStock()
		{
			var resourceType = BlobsMemory.Instance.ReferencesOf<ResourceTypeBlob>()[0];

			var stockEntity = _entityManager.CreateEntity( typeof( Stock ), typeof( StockCountChange ) );
			_entityManager.SetComponentData( stockEntity, new Stock() { Capacity = 50, Count = 45, Type = resourceType } );
			_entityManager.SetComponentData( stockEntity, new StockCountChange() { Count = 10, Type = resourceType } );

			Update();

			Assert.AreEqual( 50, _entityManager.GetComponentData<Stock>( stockEntity ).Count );
			Assert.IsFalse( _entityManager.HasComponent<StockCountChange>( stockEntity ) );
		}

		[Test]
		public void SimpleUpdate_CreateStock()
		{
			var resourceType = BlobsMemory.Instance.ReferencesOf<ResourceTypeBlob>()[0];

			var stockEntity = _entityManager.CreateEntity( typeof( StockCountChange ) );
			_entityManager.SetComponentData( stockEntity, new StockCountChange() { Count = 1, Type = resourceType } );

			Update();

			Assert.IsTrue( _entityManager.HasComponent<Stock>( stockEntity ) );
			Assert.AreEqual( 1, _entityManager.GetComponentData<Stock>( stockEntity ).Count );
		}
	}
}
