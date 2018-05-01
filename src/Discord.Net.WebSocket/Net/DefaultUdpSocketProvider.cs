using System;

namespace Discord.Net.Udp
{
    public static class DefaultUdpSocketProvider
    {
        public static readonly UdpSocketProvider Instance = () => 
        {
            try
            {
                return new DefaultUdpSocket();
            }
            catch (PlatformNotSupportedException ex)
            {
                throw new PlatformNotSupportedException("The default UdpSocketProvider is not supported on this platform.", ex);
            }
        };
    }
}
