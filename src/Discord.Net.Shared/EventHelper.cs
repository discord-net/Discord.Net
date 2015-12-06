using System;

namespace Discord
{
    internal static class EventHelper
	{
		public static void Raise(Logger logger, string name, Action action)
		{
			try { action(); }
			catch (Exception ex)
			{
				var ex2 = ex.GetBaseException();
				logger.Log(LogSeverity.Error, $"{name}'s handler raised {ex2.GetType().Name}: ${ex2.Message}", ex);
			}
		}
	}
}
