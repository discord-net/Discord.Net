using Discord.Net.Udp;

namespace Discord.Net.Providers.UnstableUdpSocket
{
    public static class UnstableUdpSocketProvider
    {
        public static readonly UdpSocketProvider Instance = () => new UnstableUdpSocket();
    }
}
