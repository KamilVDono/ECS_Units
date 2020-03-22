using NUnit.Framework;

namespace Tests.Categories
{
	public class ECSTestAttribute : CategoryAttribute
	{
	}

	public class ECS_INVALID_TestAttribute : CategoryAttribute
	{
		public string Reason;

		public ECS_INVALID_TestAttribute( string reason ) => Reason = reason;
	}

	public class UtilsTestAttribute : CategoryAttribute
	{
	}
}