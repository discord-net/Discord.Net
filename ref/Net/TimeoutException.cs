using System;

namespace Discord.Net
{
    public class TimeoutException : OperationCanceledException
	{
		public TimeoutException() { }
	}
}
