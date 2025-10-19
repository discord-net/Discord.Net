using Discord.Models;

namespace Discord;

public interface IMessage :
    IEntity<Snowflake, IMessageModel>,
    IMessageActor
{
    IUserActor Author { get; }

    IReadOnlyList<IUserActor> MentionedUsers { get; }

    IReadOnlyList<IChannelActor> MentionedChannels { get; }

    IReadOnlyList<IAttachment> Attachments { get; }
}

public static class MessageExtensions
{
    extension(IMessage message)
    {
        public IReadOnlyList<Snowflake> MentionedRoles => [..message.Model.MentionRoles.Select(x => x.Id)];
        public string Content => message.Model.Content;
        
        public DateTimeOffset Timestamp => message.Model.Timestamp;
        
        public DateTimeOffset? EditedTimestamp => message.Model.EditedTimestamp;

        public bool WasEdited => message.EditedTimestamp.HasValue;

        public bool IsTTS => message.Model.TTS;

        public bool MentionsEveryone => message.Model.MentionEveryone;

        public IReadOnlyList<Embed> Embeds => [..message.Model.Embeds.Select(Embed.FromModel)];
    }
}