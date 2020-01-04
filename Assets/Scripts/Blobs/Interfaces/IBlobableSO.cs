using System;

using Unity.Entities;

namespace Blobs.Interfaces
{
	// Mark SO which is able to be converted to blob data
	public interface IBlobableSO : IDisposable
	{
		Type BlobType { get; }

		void SetupBlobReference();
	}

	public interface IBlobableSO<T> : IBlobableSO where T : struct, IBlobable
	{
		BlobAssetReference<T> BlobReference { get; }
	}
}