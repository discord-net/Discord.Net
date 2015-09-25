using System;

namespace Discord
{
	public sealed class TimeoutException : Exception
	{
		internal TimeoutException()
			: base("An operation has timed out.")
		{
		}
	}
}
