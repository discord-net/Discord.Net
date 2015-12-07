using System;

namespace Discord.Net
{
	public sealed class TimeoutException : OperationCanceledException
	{
		public TimeoutException()
			: base("An operation has timed out.")
		{
		}
	}
}
