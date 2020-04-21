using Blobs.Interfaces;

using System.Linq;

using UnityEngine;

namespace Blobs
{
	public class BlobsMemoryAuthoring : MonoBehaviour
	{
		[SerializeField][OnlySizeArray] private NamedGroup[] _namedGroups = new NamedGroup[0];

		private void Awake() => BlobsMemory.FromSOs( _namedGroups.SelectMany( ng => ng.BlobableSOs ).OfType<IBlobableSO>().ToArray() );
	}

	[System.Serializable]
	public class NamedGroup
	{
		public string Name;
		public ScriptableObject[] BlobableSOs;
	}

	public class OnlySizeArrayAttribute : PropertyAttribute { }
}