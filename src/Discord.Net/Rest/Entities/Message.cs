using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Message : IMessage
    {
        /// <inheritdoc />
        public ulong Id { get; }

        /// <inheritdoc />
        public DateTime? EditedTimestamp { get; private set; }
        /// <inheritdoc />
        public bool IsTTS { get; private set; }
        /// <inheritdoc />
        public string RawText { get; private set; }
        /// <inheritdoc />
        public string Text { get; private set; }
        /// <inheritdoc />
        public DateTime Timestamp { get; private set; }

        /// <inheritdoc />
        public IMessageChannel Channel { get; }
        /// <inheritdoc />
        public IUser Author { get; }

        /// <inheritdoc />
        public IReadOnlyList<Attachment> Attachments { get; private set; }
        /// <inheritdoc />
        public IReadOnlyList<Embed> Embeds { get; private set; }
        /// <inheritdoc />
        public IReadOnlyList<IUser> MentionedUsers { get; private set; }
        /// <inheritdoc />
        public IReadOnlyList<ulong> MentionedChannelIds { get; private set; }
        /// <inheritdoc />
        public IReadOnlyList<ulong> MentionedRoleIds { get; private set; }

        /// <inheritdoc />
        public DateTime CreatedAt => DateTimeUtils.FromSnowflake(Id);
        internal DiscordClient Discord => (Channel as TextChannel)?.Discord ?? (Channel as DMChannel).Discord;

        internal Message(IMessageChannel channel, Model model)
        {
            Id = model.Id;
            Channel = channel;
            Author = new PublicUser(Discord, model.Author);

            Update(model);
        }
        private void Update(Model model)
        {
            var guildChannel = Channel as GuildChannel;
            var guild = guildChannel?.Guild;
            var discord = Discord;

            IsTTS = model.IsTextToSpeech;
            Timestamp = model.Timestamp;
            EditedTimestamp = model.EditedTimestamp;
            RawText = model.Content;

            if (model.Attachments.Length > 0)
            {
                var attachments = new Attachment[model.Attachments.Length];
                for (int i = 0; i < attachments.Length; i++)
                    attachments[i] = new Attachment(model.Attachments[i]);
                Attachments = ImmutableArray.Create(attachments);
            }
            else
                Attachments = Array.Empty<Attachment>();

            if (model.Embeds.Length > 0)
            {
                var embeds = new Embed[model.Attachments.Length];
                for (int i = 0; i < embeds.Length; i++)
                    embeds[i] = new Embed(model.Embeds[i]);
                Embeds = ImmutableArray.Create(embeds);
            }
            else
                Embeds = Array.Empty<Embed>();

            if (guildChannel != null && model.Mentions.Length > 0)
            {
                var mentions = new PublicUser[model.Mentions.Length];
                for (int i = 0; i < model.Mentions.Length; i++)
                    mentions[i] = new PublicUser(discord, model.Mentions[i]);
                MentionedUsers = ImmutableArray.Create(mentions);
            }
            else
                MentionedUsers = Array.Empty<PublicUser>();

            if (guildChannel != null)
            {
                MentionedChannelIds = MentionUtils.GetChannelMentions(model.Content);

                var mentionedRoleIds = MentionUtils.GetRoleMentions(model.Content);
                if (model.IsMentioningEveryone)
                    mentionedRoleIds = mentionedRoleIds.Add(guildChannel.Guild.EveryoneRole.Id);
                MentionedRoleIds = mentionedRoleIds;
            }
            else
            {
                MentionedChannelIds = Array.Empty<ulong>();
                MentionedRoleIds = Array.Empty<ulong>();
            }
            
            Text = MentionUtils.CleanUserMentions(model.Content, model.Mentions);
        }

        /// <inheritdoc />
        public async Task Modify(Action<ModifyMessageParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyMessageParams();
            func(args);
            var guildChannel = Channel as GuildChannel;

            Model model;
            if (guildChannel != null)
                model = await Discord.ApiClient.ModifyMessage(guildChannel.Guild.Id, Channel.Id, Id, args).ConfigureAwait(false);
            else
                model = await Discord.ApiClient.ModifyDMMessage(Channel.Id, Id, args).ConfigureAwait(false);
            Update(model);
        }

        /// <inheritdoc />
        public async Task Delete()
        {
            var guildChannel = Channel as GuildChannel;
            if (guildChannel != null)
                await Discord.ApiClient.DeleteMessage(guildChannel.Id, Channel.Id, Id).ConfigureAwait(false);
            else
                await Discord.ApiClient.DeleteDMMessage(Channel.Id, Id).ConfigureAwait(false);
        }

        public override string ToString() => Text;
        private string DebuggerDisplay => $"{Author}: {Text}{(Attachments.Count > 0 ? $" [{Attachments.Count} Attachments]" : "")}";

        IUser IMessage.Author => Author;
        IReadOnlyList<IUser> IMessage.MentionedUsers => MentionedUsers;
    }
}
