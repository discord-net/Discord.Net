using System;

namespace Discord.Net
{
#if NET46
    [Serializable]
#endif
    public sealed class TimeoutException : OperationCanceledException
	{
		public TimeoutException()
			: base("An operation has timed out.")
		{
		}
	}
}
