using Helpers;

using Unity.Entities;
using Unity.Mathematics;

namespace PathFinding.Components
{
	public struct PathRequest : IComponentData
	{
		public Boolean Done;
		public int2 End;
		public int2 Start;

		public PathRequest( int2 start, int2 end ) : this()
		{
			this.Start = start;
			this.End = end;
		}
	}
}