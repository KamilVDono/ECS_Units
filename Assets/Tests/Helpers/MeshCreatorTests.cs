using NUnit.Framework;

using Tests.Categories;

using static Helpers.MeshCreator;
using static NUnit.Framework.Assert;

namespace Tests.Assets.Tests.Helpers
{
	public class MeshCreatorTests
	{
		public class Quad_Tests
		{
			[Test]
			[UtilsTest]
			public void SimpleQuad()
			{
				var quad = Quad(1);

				NotNull( quad );
			}
		}
	}
}