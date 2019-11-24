using Helpers.Assets.Scripts.Helpers;
using Maps.Authoring;
using NUnit.Framework;
using UnityEngine;
using static NUnit.Framework.Assert;

namespace Tests.Helpers
{
	public class ResourcePathTest
	{
		private TileTypeSO SandTileSO => ResourcePath.TILES_SO.Load<TileTypeSO>( "Sand" );

		[Test]
		public void LoadOneTile_Loaded() => IsNotNull( SandTileSO );

		[Test]
		public void LoadOneTile_SandIsSand() => AreEqual( "Sand", SandTileSO.name );

		[Test]
		public void LoadAllTiles()
		{
			TileTypeSO[] AllTileSO = ResourcePath.TILES_SO.All<TileTypeSO>();
			IsNotNull( AllTileSO );
			Greater( AllTileSO.Length, 0 );
		}

		[Test]
		public void LoadAllMaterials()
		{
			Material[] AllTileSO = ResourcePath.TILES_MATERIAL.All<Material>();
			IsNotNull( AllTileSO );
			Greater( AllTileSO.Length, 0 );
		}


	}
}
