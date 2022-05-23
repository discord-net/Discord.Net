using Newtonsoft.Json;
using System;
using System.Linq;

namespace Discord.API
{
    internal class Message : IMessageModel
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("type")]
        public MessageType Type { get; set; }
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        // ALWAYS sent on WebSocket messages
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonProperty("webhook_id")]
        public Optional<ulong> WebhookId { get; set; }
        [JsonProperty("author")]
        public Optional<User> Author { get; set; }
        // ALWAYS sent on WebSocket messages
        [JsonProperty("member")]
        public Optional<GuildMember> Member { get; set; }
        [JsonProperty("content")]
        public Optional<string> Content { get; set; }
        [JsonProperty("timestamp")]
        public Optional<DateTimeOffset> Timestamp { get; set; }
        [JsonProperty("edited_timestamp")]
        public Optional<DateTimeOffset?> EditedTimestamp { get; set; }
        [JsonProperty("tts")]
        public Optional<bool> IsTextToSpeech { get; set; }
        [JsonProperty("mention_everyone")]
        public Optional<bool> MentionEveryone { get; set; }
        [JsonProperty("mentions")]
        public Optional<User[]> UserMentions { get; set; }
        [JsonProperty("mention_roles")]
        public Optional<ulong[]> RoleMentions { get; set; }
        [JsonProperty("attachments")]
        public Optional<Attachment[]> Attachments { get; set; }
        [JsonProperty("embeds")]
        public Optional<Embed[]> Embeds { get; set; }
        [JsonProperty("pinned")]
        public Optional<bool> Pinned { get; set; }
        [JsonProperty("reactions")]
        public Optional<Reaction[]> Reactions { get; set; }
        // sent with Rich Presence-related chat embeds
        [JsonProperty("activity")]
        public Optional<MessageActivity> Activity { get; set; }
        // sent with Rich Presence-related chat embeds
        [JsonProperty("application")]
        public Optional<MessageApplication> Application { get; set; }
        [JsonProperty("application_id")]
        public Optional<ulong> ApplicationId { get; set; }
        [JsonProperty("message_reference")]
        public Optional<MessageReference> Reference { get; set; }
        [JsonProperty("flags")]
        public Optional<MessageFlags> Flags { get; set; }
        [JsonProperty("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }
        [JsonProperty("referenced_message")]
        public Optional<Message> ReferencedMessage { get; set; }
        [JsonProperty("components")]
        public Optional<API.ActionRowComponent[]> Components { get; set; }
        public Optional<MessageInteraction> Interaction { get; set; }
        [JsonProperty("sticker_items")]
        public Optional<StickerItem[]> StickerItems { get; set; }


        MessageType IMessageModel.Type { get => Type; set => throw new NotSupportedException(); }
        ulong IMessageModel.ChannelId { get => ChannelId; set => throw new NotSupportedException(); }
        ulong? IMessageModel.GuildId { get => GuildId.ToNullable(); set => throw new NotSupportedException(); }
        ulong IMessageModel.AuthorId { get => Author.IsSpecified ? Author.Value.Id : Member.IsSpecified ? Member.Value.User.Id : WebhookId.GetValueOrDefault(); set => throw new NotSupportedException(); }
        bool IMessageModel.IsWebhookMessage { get => WebhookId.IsSpecified; set => throw new NotSupportedException(); }
        string IMessageModel.Content { get => Content.GetValueOrDefault(); set => throw new NotSupportedException(); }
        DateTimeOffset IMessageModel.Timestamp { get => Timestamp.Value; set => throw new NotSupportedException(); } // might break?
        DateTimeOffset? IMessageModel.EditedTimestamp { get => Timestamp.ToNullable(); set => throw new NotSupportedException(); }
        bool IMessageModel.IsTextToSpeech { get => IsTextToSpeech.GetValueOrDefault(); set => throw new NotSupportedException(); }
        bool IMessageModel.MentionEveryone { get => MentionEveryone.GetValueOrDefault(); set => throw new NotSupportedException(); }
        ulong[] IMessageModel.UserMentionIds { get => UserMentions.IsSpecified ? UserMentions.Value.Select(x => x.Id).ToArray() : Array.Empty<ulong>(); set => throw new NotSupportedException(); }
        IAttachmentModel[] IMessageModel.Attachments { get => Attachments.GetValueOrDefault(Array.Empty<Attachment>()); set => throw new NotSupportedException(); }
        IEmbedModel[] IMessageModel.Embeds { get => Embeds.GetValueOrDefault(Array.Empty<Embed>()); set => throw new NotSupportedException(); }
        IReactionMetadataModel[] IMessageModel.Reactions { get => Reactions.GetValueOrDefault(Array.Empty<Reaction>()); set => throw new NotSupportedException(); }
        bool IMessageModel.Pinned { get => Pinned.GetValueOrDefault(); set => throw new NotSupportedException(); }
        IMessageActivityModel IMessageModel.Activity { get => Activity.GetValueOrDefault(); set => throw new NotSupportedException(); }
        IPartialApplicationModel IMessageModel.Application { get => Application.GetValueOrDefault(); set => throw new NotSupportedException(); }
        ulong? IMessageModel.ApplicationId { get => ApplicationId.ToNullable(); set => throw new NotSupportedException(); }
        ulong? IMessageModel.ReferenceMessageId { get => ReferencedMessage.GetValueOrDefault()?.Id; set => throw new NotSupportedException(); }
        ulong? IMessageModel.ReferenceMessageChannelId { get => ReferencedMessage.GetValueOrDefault()?.ChannelId; set => throw new NotSupportedException(); }
        MessageFlags IMessageModel.Flags { get => Flags.GetValueOrDefault(); set => throw new NotSupportedException(); }
        ulong? IMessageModel.InteractionId { get => Interaction.GetValueOrDefault()?.Id; set => throw new NotSupportedException(); }
        string IMessageModel.InteractionName { get => Interaction.GetValueOrDefault()?.Name; set => throw new NotSupportedException(); }
        InteractionType? IMessageModel.InteractionType { get => Interaction.GetValueOrDefault()?.Type; set => throw new NotSupportedException(); }
        ulong? IMessageModel.InteractionUserId { get => Interaction.GetValueOrDefault()?.User.Id; set => throw new NotSupportedException(); }
        IMessageComponentModel[] IMessageModel.Components { get => Components.GetValueOrDefault(Array.Empty<ActionRowComponent>()); set => throw new NotSupportedException(); }
        IStickerItemModel[] IMessageModel.Stickers { get => StickerItems.GetValueOrDefault(Array.Empty<StickerItem>()); set => throw new NotSupportedException(); }
        ulong IEntityModel<ulong>.Id { get => Id; set => throw new NotSupportedException(); }
    }
}
