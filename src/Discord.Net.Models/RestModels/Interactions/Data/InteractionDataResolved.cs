using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InteractionDataResolved : IEntityModelSource
{
    [JsonPropertyName("users")]
    public Optional<Dictionary<string, User>> Users { get; set; }

    [JsonPropertyName("members")]
    public Optional<Dictionary<string, GuildMember>> Members { get; set; }

    [JsonPropertyName("roles")]
    public Optional<Dictionary<string, Role>> Roles { get; set; }

    [JsonPropertyName("channels")]
    public Optional<Dictionary<string, Channel>> Channels { get; set; }

    [JsonPropertyName("messages")]
    public Optional<Dictionary<string, Message>> Messages { get; set; }

    [JsonPropertyName("attachments")]
    public Optional<Dictionary<string, Attachment>> Attachments { get; set; }

    public IEnumerable<IEntityModel> GetEntities()
    {
        if(Users.IsSpecified) foreach (var (_, entity) in Users.Value)
            yield return entity;

        if(Members.IsSpecified) foreach (var (_, entity) in Members.Value)
            yield return entity;

        if(Roles.IsSpecified) foreach (var (_, entity) in Roles.Value)
            yield return entity;

        if(Channels.IsSpecified) foreach (var (_, entity) in Channels.Value)
            yield return entity;

        if(Messages.IsSpecified) foreach (var (_, entity) in Messages.Value)
            yield return entity;

        if(Attachments.IsSpecified) foreach (var (_, entity) in Attachments.Value)
            yield return entity;
    }
}
