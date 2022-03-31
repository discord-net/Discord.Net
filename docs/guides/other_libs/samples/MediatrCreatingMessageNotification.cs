// MessageReceivedNotification.cs

using Discord.WebSocket;
using MediatR;

namespace Viscoin.Bot.Infrastructure.Messages;

public class MessageReceivedNotification : INotification
{
    public MessageReceivedNotification(SocketMessage message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    public SocketMessage Message { get; }
}
