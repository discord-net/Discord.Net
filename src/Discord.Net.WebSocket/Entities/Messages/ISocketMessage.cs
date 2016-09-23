using Model = Discord.API.Message;

namespace Discord.WebSocket
{
    internal interface ISocketMessage : IMessage
    {
        DiscordSocketClient Discord { get; }
        new ISocketMessageChannel Channel { get; }

        void Update(Model model);
        ISocketMessage Clone();
    }
}
