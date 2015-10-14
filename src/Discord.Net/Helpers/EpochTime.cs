using System;

namespace Discord.Helpers
{
    public class EpochTime
	{
		private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		public static ulong GetMilliseconds() => (ulong)(DateTime.UtcNow - epoch).TotalMilliseconds;
	}
}
