using Blobs.Interfaces;

using System.Linq;

using UnityEngine;

namespace Blobs
{
	public class BlobsMemoryAuthoring : MonoBehaviour
	{
		[SerializeField] private NamedGroup[] _namedGroups;

		private void Awake() => BlobsMemory.FromSOs( _namedGroups.SelectMany( ng => ng.BlobableSOs ).OfType<IBlobableSO>().ToArray() );
	}

	[System.Serializable]
	public class NamedGroup
	{
		public string Name;
		public ScriptableObject[] BlobableSOs;
	}
}