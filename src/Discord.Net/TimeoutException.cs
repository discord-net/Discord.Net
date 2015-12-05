using System;

namespace Discord
{
	public sealed class TimeoutException : OperationCanceledException
	{
		public TimeoutException()
			: base("An operation has timed out.")
		{
		}
	}
}
