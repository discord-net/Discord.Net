using Discord.Models.Json;

namespace Discord;

public sealed class ModifyWebhookMessageProperties : IEntityProperties<ModifyWebhookMessageParams>
{
    public Optional<string> Content { get; set; }
    public Optional<Embed[]> Embeds { get; set; }
    public Optional<AllowedMentions> AllowedMentions { get; set; }
    public Optional<IMessageComponent[]> Components { get; set; }
    public Optional<FileAttachment[]> Attachments { get; set; }

    public ModifyWebhookMessageParams ToApiModel(ModifyWebhookMessageParams? existing = default)
    {
        existing ??= new ModifyWebhookMessageParams();

        existing.Content = Content;
        existing.Embeds = Embeds.Map(v => v.Select(x => x.ToApiModel()).ToArray());
        existing.AllowedMentions = AllowedMentions.Map(v => v.ToApiModel());
        existing.Components = Components.Map(v => v.Select(x => x.ToApiModel()).ToArray());
        existing.Attachments = Attachments.Map(v => v.Select(x => x.ToApiModel()).ToArray());

        return existing;
    }
}
