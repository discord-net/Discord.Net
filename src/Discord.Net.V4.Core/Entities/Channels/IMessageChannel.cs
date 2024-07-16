namespace Discord;

/// <summary>
///     Represents a generic channel that can send and receive messages.
/// </summary>
public partial interface IMessageChannel :
    IChannel,
    IMessageChannelActor;
