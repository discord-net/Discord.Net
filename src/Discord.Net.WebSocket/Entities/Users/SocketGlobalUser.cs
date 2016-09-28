namespace Discord.WebSocket
{
    internal class SocketGlobalUser : SocketUser
    {
        internal SocketGlobalUser(DiscordSocketClient discord, ulong id) 
            : base(discord, id)
        {
        }
    }
}
