using System;

namespace Discord
{
    internal class EpochTime
	{
		private const long epoch = 621355968000000000L; //1/1/1970 in Ticks

		public static long GetMilliseconds() => (DateTime.UtcNow.Ticks - epoch) / TimeSpan.TicksPerMillisecond;
	}
}
