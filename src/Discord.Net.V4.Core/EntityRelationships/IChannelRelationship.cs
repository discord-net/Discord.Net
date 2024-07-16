namespace Discord;

public interface IChannelRelationship<out TActor, out TChannel> :
    IRelationship<TActor, ulong, TChannel>
    where TActor : IActor<ulong, TChannel>
    where TChannel : IChannel
{
    TActor Channel { get; }

    TActor IRelationship<TActor, ulong, TChannel>.RelationshipActor => Channel;
}

public interface IChannelRelationship : IChannelRelationship<IChannelActor, IChannel>;
