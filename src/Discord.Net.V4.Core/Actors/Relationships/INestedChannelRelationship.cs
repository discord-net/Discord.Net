namespace Discord;

public interface INestedChannelRelationship<out TActor, out TChannel> :
    IRelationship<TActor, ulong, TChannel>
    where TActor : IActor<ulong, TChannel>
    where TChannel : IChannel
{
    TActor Parent { get; }

    TActor IRelationship<TActor, ulong, TChannel>.RelationshipActor => Parent;
}

public interface INestedChannelRelationship : INestedChannelRelationship<IChannelActor, IChannel>;
