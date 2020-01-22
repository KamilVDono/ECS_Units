using System.Collections;

using Unity.PerformanceTesting;

using UnityEngine.TestTools;

namespace Tests.Performance
{
	public class MapRenderPerformance
	{
		[Performance]
		[UnityTest]
		[Version( "1" )]
		public IEnumerator MainTest()
		{
			yield return null;
			yield return null;

			yield return Measure.Frames().WarmupCount( 10 )
				.MeasurementCount( 15 ).Run();
		}
	}
}