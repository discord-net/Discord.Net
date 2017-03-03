using System;

namespace Discord.Net.Udp
{
    public static class DefaultUdpSocketProvider
    {
#if NETSTANDARD1_3
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
#else
        public static readonly UdpSocketProvider Instance = () => 
        {
            throw new PlatformNotSupportedException("The default UdpSocketProvider is not supported on this platform.\n" +
                "You must specify a UdpSocketProvider or target a runtime supporting .NET Standard 1.3, such as .NET Framework 4.6+.");
        };
#endif
    }
}