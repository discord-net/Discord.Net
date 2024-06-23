namespace Discord;

public interface IMessageChannelRelationship :
    IRelationship<ulong, IMessageChannel, ILoadableMessageChannelActor>
{
    ILoadableMessageChannelActor Channel { get; }

    ILoadableMessageChannelActor IRelationship<ulong, IMessageChannel, ILoadableMessageChannelActor>.
        RelationshipLoadable => Channel;
}
