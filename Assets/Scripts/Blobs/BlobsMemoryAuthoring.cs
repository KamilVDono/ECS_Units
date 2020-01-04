using Blobs.Interfaces;

using System.Linq;

using UnityEngine;

namespace Blobs
{
	public class BlobsMemoryAuthoring : MonoBehaviour
	{
		[SerializeField] private ScriptableObject[] _blobableSOs;

		private void Awake() => BlobsMemory.FromSOs( _blobableSOs.OfType<IBlobableSO>().ToArray() );
	}
}