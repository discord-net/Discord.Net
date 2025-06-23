using Discord.Providers.WS4Net;
using Discord.Providers.UDPClient;
using Discord.WebSocket;
// ...
var client = new DiscordSocketClient(new DiscordSocketConfig 
{
	WebSocketProvider = WS4NetProvider.Instance,
	UdpSocketProvider = UDPClientProvider.Instance,
});