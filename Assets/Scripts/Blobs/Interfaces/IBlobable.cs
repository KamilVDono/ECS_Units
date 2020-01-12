namespace Blobs.Interfaces
{
	// Mark struct which is able to be blob data
	public interface IBlobable
	{
	}

	public interface IBlobable<T> : IBlobable where T : IBlobableSO
	{
	}
}