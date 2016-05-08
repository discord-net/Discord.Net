using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.Rest
{
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
        public User Author { get; }

        /// <inheritdoc />
        public IReadOnlyList<Attachment> Attachments { get; private set; }
        /// <inheritdoc />
        public IReadOnlyList<Embed> Embeds { get; private set; }
        /// <inheritdoc />
        public IReadOnlyList<PublicUser> MentionedUsers { get; private set; }
        /// <inheritdoc />
        public IReadOnlyList<ulong> MentionedChannelIds { get; private set; }
        /// <inheritdoc />
        public IReadOnlyList<ulong> MentionedRoleIds { get; private set; }

        /// <inheritdoc />
        public DateTime CreatedAt => DateTimeHelper.FromSnowflake(Id);
        /// <inheritdoc />
        public bool IsAuthor => Discord.CurrentUser.Id == Author.Id;
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
                Attachments = ImmutableArray<Attachment>.Empty;

            if (model.Embeds.Length > 0)
            {
                var embeds = new Embed[model.Attachments.Length];
                for (int i = 0; i < embeds.Length; i++)
                    embeds[i] = new Embed(model.Embeds[i]);
                Embeds = ImmutableArray.Create(embeds);
            }
            else
                Embeds = ImmutableArray<Embed>.Empty;

            if (model.Mentions.Length > 0)
            {
                var discord = Discord;
                var builder = ImmutableArray.CreateBuilder<PublicUser>(model.Mentions.Length);
                for (int i = 0; i < model.Mentions.Length; i++)
                    builder[i] = new PublicUser(discord, model.Mentions[i]);
                MentionedUsers = builder.ToArray();
            }
            else
                MentionedUsers = ImmutableArray<PublicUser>.Empty;
            MentionedChannelIds = MentionHelper.GetChannelMentions(model.Content);
            MentionedRoleIds = MentionHelper.GetRoleMentions(model.Content);
            if (model.IsMentioningEveryone)
            {
                ulong? guildId = (Channel as IGuildChannel)?.Guild.Id;
                if (guildId != null)
                {
                    if (MentionedRoleIds.Count == 0)
                        MentionedRoleIds = ImmutableArray.Create(guildId.Value);
                    else
                    {
                        var builder = ImmutableArray.CreateBuilder<ulong>(MentionedRoleIds.Count + 1);
                        builder.AddRange(MentionedRoleIds);
                        builder.Add(guildId.Value);
                        MentionedRoleIds = builder.ToImmutable();
                    }
                }
            }
            
            Text = MentionHelper.CleanUserMentions(model.Content, model.Mentions);

            Author.Update(model.Author);
        }

        /// <inheritdoc />
        public async Task Modify(Action<ModifyMessageParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyMessageParams();
            func(args);
            var model = await Discord.BaseClient.ModifyMessage(Channel.Id, Id, args).ConfigureAwait(false);
            Update(model);
        }

        /// <inheritdoc />
        public async Task Delete()
        {
            await Discord.BaseClient.DeleteMessage(Channel.Id, Id).ConfigureAwait(false);
        }

        IUser IMessage.Author => Author;
        IReadOnlyList<Attachment> IMessage.Attachments => Attachments;
        IReadOnlyList<Embed> IMessage.Embeds => Embeds;
        IReadOnlyList<ulong> IMessage.MentionedChannelIds => MentionedChannelIds;
        IReadOnlyList<IUser> IMessage.MentionedUsers => MentionedUsers;
    }
}
