using Discord.Models.Json;

namespace Discord;

[Flags]
public enum InteractionCallbackFlags : int
{
    SupressEmbed = 1 << 2,
    Ephemeral = 1 << 6,
    SuppressNotifications = 1 << 12
}

public class InteractionMessageCallbackDataProperties : 
    ICallbackDataProperties
{
    public Optional<bool> IsTTS { get; set; }
    public Optional<string> Content { get; set; }
    public Optional<IEnumerable<Embed>> Embeds { get; set; }
    public Optional<AllowedMentions> AllowedMentions { get; set; }
    public Optional<InteractionCallbackFlags> Flags { get; set; }
    public Optional<IEnumerable<IMessageComponent>> Components { get; set; }
    public Optional<IEnumerable<FileAttachment>> Attachments { get; set; }
    // TODO: poll
    
    public InteractionCallbackData ToApiModel(InteractionCallbackData? existing = default)
    {
        return new InteractionCallbackData()
        {
            TTS = IsTTS,
            Content = Content,
            Embeds = Embeds.Map(v => v.Select(v => v.ToApiModel()).ToArray()),
            AllowedMentions = AllowedMentions.Map(v => v.ToApiModel()),
            Flags = Flags.MapToInt(),
            Components = Components.Map(v => v.Select(v => v.ToApiModel()).ToArray()),
            Attachments = Attachments.Map(v => v.Select(v => v.ToApiModel()).ToArray())
        };
    }
}