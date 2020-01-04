using Blobs.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Entities;

namespace Blobs
{
	public class BlobsMemory : IDisposable
	{
		#region Singleton
		public static BlobsMemory Instance { get; private set; }

		public static BlobsMemory FromSOs( IBlobableSO[] blobables )
		{
			if ( Instance != null )
			{
				Instance.Dispose();
			}
			var newInstance = new BlobsMemory
			{
				_blobables = blobables
			};
			newInstance.CreateBlobsData();
			Instance = newInstance;

			return Instance;
		}

		#endregion Singleton

		private IBlobableSO[] _blobables;
		private Dictionary<Type, IBlobableSO[]> _blobsCache = new Dictionary<Type, IBlobableSO[]>();

		public BlobAssetReference<T>[] ReferencesOf<T>() where T : struct, IBlobable
		{
			if ( _blobsCache.TryGetValue( typeof( T ), out var scriptableObjects ) )
			{
				return scriptableObjects.Select( so => ( so as IBlobableSO<T> ).BlobReference ).ToArray();
			}

			return null;
		}

		public void Dispose()
		{
			foreach ( var blobable in _blobables )
			{
				blobable.Dispose();
			}
			Instance = null;
			_blobables = null;
			_blobsCache = null;
		}

		private void CreateBlobsData()
		{
			// Clear previous cache
			foreach ( var blobable in _blobables )
			{
				blobable.SetupBlobReference();
			}
			// Aggregate to dictionary
			_blobsCache = _blobables.GroupBy( so => so.BlobType ).ToDictionary( g => g.Key, g => g.ToArray() );
		}
	}
}