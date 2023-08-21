using System;
namespace Discord.Gateway
{
	public sealed record ETFPID(
		object? Node,
		int Id,
		int Serial,
		byte Creation);
}

