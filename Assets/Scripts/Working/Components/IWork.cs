using Unity.Entities;

namespace Working.Components
{
	public interface IWork : IComponentData
	{
		float ProgressPerSecond { get; }
	}
}