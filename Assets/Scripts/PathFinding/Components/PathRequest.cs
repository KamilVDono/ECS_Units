using Helpers.Types;

using Unity.Entities;
using Unity.Mathematics;

namespace Pathfinding.Components
{
	public struct PathRequest : IComponentData
	{
		public int2 End;
		public int2 Start;

		public PathRequest( int2 start, int2 end ) : this()
		{
			this.Start = start;
			this.End = end;
		}
	}

	public struct PathRequestBuilder : IComponentData
	{
		public int2 Start;
		public Boolean Started;
	}
}