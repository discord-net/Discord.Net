using System.Collections;
using Discord.Models.Json;

namespace Discord;

[Flags]
public enum MessageCreateFlags
{
    /// <summary>
    ///     Flag given to messages that do not display any embeds.
    /// </summary>
    SuppressEmbeds = 1 << 2,
    
    /// <summary>
    ///     Flag give to messages that will not trigger push and desktop notifications.
    /// </summary>
    SuppressNotification = 1 << 12,
}

public sealed class CreateMessageProperties : IEntityProperties<CreateMessageParams>
{
    public Optional<string> Content { get; set; }
    public Optional<string> Nonce { get; set; }
    public Optional<bool> TextToSpeech { get; set; }
    public Optional<ICollection<Embed>> Embeds { get; set; }
    public Optional<AllowedMentions> AllowedMentions { get; set; }
    public Optional<MessageReference> MessageReference { get; set; }
    public Optional<ICollection<IMessageComponent>> Components { get; set; }
    public Optional<IEnumerable<EntityOrId<ulong, ISticker>>> Stickers { get; set; }
    public Optional<IEnumerable<FileAttachment>> Attachments { get; set; }
    public Optional<MessageCreateFlags> Flags { get; set; }
    public Optional<bool> EnforceNonce { get; set; }
    
    // TODO: poll
    
    public CreateMessageParams ToApiModel(CreateMessageParams? existing = default)
    {
        return new CreateMessageParams()
        {
            Flags = Flags.MapToInt(),
            Attachments = Attachments.Map(v => v.Select(x => x.ToApiModel()).ToArray()),
            Components = Components.Map(v => v.Select(x => x.ToApiModel()).ToArray()),
            Content = Content,
            Embeds = Embeds.Map(v => v.Select(x => x.ToApiModel()).ToArray()),
            Nonce = Nonce,
            AllowedMentions = AllowedMentions.Map(v => v.ToApiModel()),
            MessageReference = MessageReference.Map(v => v.ToApiModel()),
            IsTTS = TextToSpeech,
            StickerIds = Stickers.Map(v => v.Select(x => x.Id).ToArray())
        };
    }
}
