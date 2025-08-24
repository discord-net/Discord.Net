using Discord.Models;

namespace Discord.Rest;

public sealed class RestGuildMessage : 
    RestMessage,
    IGuildMessage
{
    public RestGuildMessage(DiscordRestClient client, IMessageModel model, RestMessageActor? actor = null) : base(client, model, actor)
    {
    }

    public IReadOnlyList<IRoleActor> MentionedRoles => throw new NotImplementedException();
}