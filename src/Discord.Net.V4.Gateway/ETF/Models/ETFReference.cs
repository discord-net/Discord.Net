using System;
namespace Discord.Gateway
{
	public sealed record ETFReference(
		object? Node,
		byte Creation,
		int[] Ids);
}

