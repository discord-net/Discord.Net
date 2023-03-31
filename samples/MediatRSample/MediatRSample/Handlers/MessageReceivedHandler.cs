using MediatR;
using MediatRSample.Notifications;

namespace MediatRSample.Handlers;

public class MessageReceivedHandler : INotificationHandler<MessageReceivedNotification>
{
    public async Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"MediatR works! (Received a message by {notification.Message.Author.Username})");
        
        // Your implementation
    }
}