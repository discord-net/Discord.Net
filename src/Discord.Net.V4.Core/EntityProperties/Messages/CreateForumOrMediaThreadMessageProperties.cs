using Discord.Models.Json;

namespace Discord;

public class CreateForumOrMediaThreadMessageProperties : IEntityProperties<ForumOrMediaThreadMessage>
{
    public Optional<string> Content { get; set; }
    public Optional<ICollection<Embed>> Embeds { get; set; }
    public Optional<AllowedMentions> AllowedMentions { get; set; }
    public Optional<ICollection<IMessageComponent>> Components { get; set; }
    public Optional<IEnumerable<EntityOrId<ulong, ISticker>>> Stickers { get; set; }
    public Optional<IEnumerable<FileAttachment>> Attachments { get; set; }
    public Optional<MessageCreateFlags> Flags { get; set; }
    
    public ForumOrMediaThreadMessage ToApiModel(ForumOrMediaThreadMessage? existing = default)
    {
        return new ForumOrMediaThreadMessage()
        {
            Flags = Flags.MapToInt(),
            Attachments = Attachments.Map(v => v.Select(x => x.ToApiModel()).ToArray()),
            Components = Components.Map(v => v.Select(x => x.ToApiModel()).ToArray()),
            Content = Content,
            Embeds = Embeds.Map(v => v.Select(x => x.ToApiModel()).ToArray()),
            AllowedMentions = AllowedMentions.Map(v => v.ToApiModel()),
            StickerIds = Stickers.Map(v => v.Select(x => x.Id).ToArray())
        };
    }
}