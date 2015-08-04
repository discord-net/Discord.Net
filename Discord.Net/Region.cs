using System;

namespace Discord
{
	public enum Region
	{
		US_West,
		US_East,
		Singapore,
		London,
		Sydney,
		Amsterdam
	}

	internal static class RegionConverter
	{
		public static string Convert(Region region)
		{
			switch (region)
			{
				case Region.US_West: return "us-west";
				case Region.US_East: return "us-east";
				case Region.Singapore: return "singapore";
				case Region.London: return "london";
				case Region.Sydney: return "sydney";
				case Region.Amsterdam: return "amsterdam";
				default:
					throw new ArgumentOutOfRangeException("Unknown server region");
			}
		}
	}
}
