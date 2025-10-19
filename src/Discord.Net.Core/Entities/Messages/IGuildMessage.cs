namespace Discord;

public interface IGuildMessage : 
    IMessage
{
    IReadOnlyList<IRoleActor> MentionedRoles { get; }
}