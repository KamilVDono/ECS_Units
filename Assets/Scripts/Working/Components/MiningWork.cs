using Unity.Entities;

namespace Working.Components
{
	public struct MiningWork : IWork
	{
		public float ProgressPerSecond { get; set; }
		public Entity Worker;
	}
}