namespace Discord;

public interface IGuildMessage : 
    IMessage
{
    new IReadOnlyList<IRoleActor> MentionedRoles { get; }

    IReadOnlyList<Snowflake> IMessage.MentionedRoles => [..MentionedRoles.Select(x => x.Id)];
}