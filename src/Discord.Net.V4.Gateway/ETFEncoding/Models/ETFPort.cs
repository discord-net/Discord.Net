using System;
namespace DiscordGatewayTest.ETF.Models
{
	public sealed record ETFPort(
		object? Node,
		int Id,
		byte Creation);
}

