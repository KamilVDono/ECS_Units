using UnityEngine;

namespace Helpers.Assets.Scripts.Helpers
{
	public class ResourcePath
	{
		public string Path { get; }

		public static readonly ResourcePath TILES_SO = new ResourcePath("ScriptableObjects/Maps/Tiles");
		public static readonly ResourcePath TILES_MATERIAL = new ResourcePath("Materials/Maps/Tiles");

		private ResourcePath( string path )
		{
			Path = path;
		}

		public string Of( string resourceName )
		{
			var resultPath = System.IO.Path.Combine(Path, resourceName);
			return resultPath;
		}

		public T Load<T>( string resourceName ) where T : Object
		{
			var fullPath = Of( resourceName );
			return Resources.Load<T>( fullPath );
		}

		public T[] All<T>() where T : Object => Resources.LoadAll<T>( Path );

		public static implicit operator string( ResourcePath resourcePath ) => resourcePath.Path;
	}
}
