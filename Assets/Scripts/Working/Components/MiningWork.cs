using Unity.Entities;

namespace Working.Components
{
	public struct MiningWork : IWork
	{
		public Entity Worker;
		public float ProgressPerSecond { get; set; }
	}
}