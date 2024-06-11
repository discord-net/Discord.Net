using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public class ModifyMessageProperties : IEntityProperties<ModifyMessageParams>
{
    public Optional<string> Content { get; set; }
    public Optional<Embed[]> Embeds { get; set; }
    public Optional<MessageFlags> Flags { get; set; }
    public Optional<AllowedMentions> AllowedMentions { get; set; }
    public Optional<IMessageComponent[]> Components { get; set; }
    public Optional<FileAttachment[]> Attachments { get; set; }

    public ModifyMessageParams ToApiModel(ModifyMessageParams? existing = default)
    {
        existing ??= new();

        existing.Content = Content;
        existing.Embeds = Embeds.Map(v => v.Select(x => x.ToApiModel()).ToArray());
        existing.Flags = Flags.Map(v => (int)v);
        existing.AllowedMentions = AllowedMentions.Map(v => v.ToApiModel());
        existing.Components = Components.Map(v => v.Select(v => v.ToApiModel()).ToArray());
        existing.Attachments = Attachments.Map(v => v.Select(v => v.ToApiModel()).ToArray());

        return existing;
    }
}
