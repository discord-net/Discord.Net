using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    internal interface ISocketChannel : IChannel
    {
        void Update(Model model, UpdateSource source);

        ISocketChannel Clone();
    }
}
