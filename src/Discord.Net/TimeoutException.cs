using System;

namespace Discord
{
	public sealed class TimeoutException : OperationCanceledException
	{
		internal TimeoutException()
			: base("An operation has timed out.")
		{
		}
	}
}
