using Discord.Rest;

namespace Discord;

public class ModifyMessageProperties
{
    public Optional<string> Content { get; set; }
    public Optional<Embed[]> Embeds { get; set; }
    public Optional<MessageFlags> Flags { get; set; }
    public Optional<AllowedMentions> AllowedMentions { get; set; }
    public Optional<IMessageComponent[]> Components { get; set; }
    public Optional<FileAttachment[]> Attachments { get; set; }
}
