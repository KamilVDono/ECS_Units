using Blobs;
using Blobs.Interfaces;

using Maps.Authoring;

using NUnit.Framework;

using Resources.Authoring;

using Tests.Utility;

using UnityEngine;

using Working.Systems;

namespace Tests.Working
{
	public class MiningWorkTest : ECSSystemTester<MiningWorkSystem>
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			TileTypeSO SandTileSO = ScriptableObject.CreateInstance<TileTypeSO>();
			SandTileSO.name = "sand";
			SandTileSO.Cost = 1f;
			SandTileSO.Color = Color.red;
			SandTileSO.Range = 1f;
			SandTileSO.hideFlags = HideFlags.HideAndDontSave;

			ResourceTypeSO coal = ScriptableObject.CreateInstance<ResourceTypeSO>();
			coal.MovementCost = 2;

			BlobsMemory.FromSOs( new IBlobableSO[] { SandTileSO, coal } );
		}

		[Test]
		public void EasyMinigWork()
		{
		}
	}
}