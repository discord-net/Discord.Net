namespace Discord;

public interface IMessageRelationship :
    IRelationship<IMessageActor, ulong, IMessage>
{
    IMessageActor Message { get; }
    
    IMessageActor IRelationship<IMessageActor, ulong, IMessage>.RelationshipActor => Message;
}