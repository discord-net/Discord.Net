using Discord.Models.Json;

namespace Discord;

[Flags]
public enum ExecuteWebhookFlags : int
{
    SuppressEmbeds = 1 << 2,
    SuppressNotifications = 1 << 12
}

public sealed class ExecuteWebhookProperties :
    IEntityProperties<ExecuteWebhookParams>
{
    public Optional<string> Content { get; set; }
    public Optional<string> Username { get; set; }
    public Optional<string> AvatarUrl { get; set; }
    public Optional<bool> IsTTS { get; set; }
    public Optional<IEnumerable<Embed>> Embeds { get; set; }
    public Optional<AllowedMentions> AllowedMentions { get; set; }
    public Optional<IEnumerable<IMessageComponent>> Components { get; set; }
    public Optional<IEnumerable<FileAttachment>> Attachments { get; set; }
    public Optional<ExecuteWebhookFlags> Flags { get; set; }
    // TODO: polls
    
    internal Optional<string> ThreadName { get; set; }
    internal Optional<IEnumerable<ulong>> Tags { get; set; }
    
    public ExecuteWebhookParams ToApiModel(ExecuteWebhookParams? existing = default)
    {
        return new ExecuteWebhookParams()
        {
            Content = Content,
            Username = Username,
            AvatarUrl = AvatarUrl,
            IsTTS = IsTTS,
            Embeds = Embeds.Map(v => v.Select(x => x.ToApiModel()).ToArray()),
            AllowedMentions = AllowedMentions.Map(v => v.ToApiModel()),
            Components = Components.Map(v => v.Select(x => x.ToApiModel()).ToArray()),
            Attachments = Attachments.Map(v => v.Select(v => v.ToApiModel()).ToArray()),
            Flags = Flags.MapToInt(),
            ThreadName = ThreadName,
            AppliedTags = Tags.Map(v => v.ToArray())
        };
    }
}