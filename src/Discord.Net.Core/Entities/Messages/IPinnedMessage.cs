using Discord.Models;

namespace Discord;

public interface IPinnedMessage : 
    IMessage,
    IPinnedMessageActor,
    IModeledBy<IMessagePinModel>
{
    new IMessagePinModel Model { get; }

    IMessageModel IModeledBy<IMessageModel>.Model => Model.Message;
    IMessagePinModel IModeledBy<IMessagePinModel>.Model => Model;
}

public static class PinnedMessageExtensions
{
    extension(IPinnedMessage message)
    {
        public DateTimeOffset PinnedAt => message.Model.PinnedAt;
    }
}