namespace Discord
{
    internal static class InternalExtensions
    {
        public static User GetCurrentUser(this IChannel channel)
        {
            switch (channel.Type)
            {
                case ChannelType.Text:
                case ChannelType.Voice:
                    return (channel as GuildChannel).Guild.CurrentUser;
                default:
                    return channel.Discord.CurrentUser;
            }
        }

/*#if NETSTANDARD1_2
        //https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/DateTimeOffset.cs
        public static long ToUnixTimeMilliseconds(this DateTimeOffset dto) => (dto.UtcDateTime.Ticks / TimeSpan.TicksPerMillisecond) - 62135596800000;
#endif*/
    }
}
