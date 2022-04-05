// MessageReceivedHandler.cs

using System;
using MediatR;
using MediatRSample.Notifications;

namespace MediatRSample;

public class MessageReceivedHandler : INotificationHandler<MessageReceivedNotification>
{
    public async Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"MediatR works! (Received a message by {notification.Message.Author.Username})");

        // Your implementation
    }
}
