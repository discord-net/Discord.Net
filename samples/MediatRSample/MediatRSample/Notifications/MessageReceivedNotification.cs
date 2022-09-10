using Discord.WebSocket;
using MediatR;

namespace MediatRSample.Notifications;

public class MessageReceivedNotification : INotification
{
    public MessageReceivedNotification(SocketMessage message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    public SocketMessage Message { get; }
}