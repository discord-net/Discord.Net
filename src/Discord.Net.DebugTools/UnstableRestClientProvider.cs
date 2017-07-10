using Discord.Net.Rest;

namespace Discord.Net.Providers.UnstableUdpSocket
{
    public static class UnstableRestClientProvider
    {
        public static readonly RestCientProvider Instance = () => new UnstableRestClientProvider();
    }
}
