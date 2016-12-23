using Discord.Net.Udp;

namespace Discord.Net.Providers.UDPClient
{
    public static class UDPClientProvider
    {
        public static readonly UdpSocketProvider Instance = () => new UDPClient();
    }
}
