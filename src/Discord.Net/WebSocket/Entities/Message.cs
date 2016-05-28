using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.WebSocket
{
    //TODO: Support mention_roles
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
        public IReadOnlyList<GuildUser> MentionedUsers { get; private set; }
        /// <inheritdoc />
        public IReadOnlyList<GuildChannel> MentionedChannels { get; private set; }
        /// <inheritdoc />
        public IReadOnlyList<Role> MentionedRoles { get; private set; }

        /// <inheritdoc />
        public DateTime CreatedAt => DateTimeUtils.FromSnowflake(Id);
        internal DiscordClient Discord => (Channel as TextChannel)?.Discord ?? (Channel as DMChannel).Discord;

        internal Message(IMessageChannel channel, IUser author, Model model)
        {
            Id = model.Id;
            Channel = channel;
            Author = author;

            Update(model);
        }
        private void Update(Model model)
        {
            var guildChannel = Channel as GuildChannel;
            var guild = guildChannel?.Guild;

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
                var builder = ImmutableArray.CreateBuilder<GuildUser>(model.Mentions.Length);
                for (int i = 0; i < model.Mentions.Length; i++)
                {
                    var user = guild.GetUser(model.Mentions[i].Id);
                    if (user != null)
                        builder.Add(user);
                }
                MentionedUsers = builder.ToArray();
            }
            else
                MentionedUsers = Array.Empty<GuildUser>();

            if (guildChannel != null/* && model.Content != null*/)
            {
                MentionedChannels = MentionUtils.GetChannelMentions(model.Content).Select(x => guild.GetChannel(x)).Where(x => x != null).ToImmutableArray();

                var mentionedRoles = MentionUtils.GetRoleMentions(model.Content).Select(x => guild.GetRole(x)).Where(x => x != null).ToImmutableArray();
                if (model.IsMentioningEveryone)
                    mentionedRoles = mentionedRoles.Add(guild.EveryoneRole);
                MentionedRoles = mentionedRoles;
            }
            else
            {
                MentionedChannels = Array.Empty<GuildChannel>();
                MentionedRoles = Array.Empty<Role>();
            }
            
            Text = MentionUtils.CleanUserMentions(model.Content, model.Mentions);

            //Author.Update(model.Author); //TODO: Uncomment this somehow
        }

        /// <inheritdoc />
        public async Task Modify(Action<ModifyMessageParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyMessageParams();
            func(args);
            var guildChannel = Channel as GuildChannel;
            
            if (guildChannel != null)
                await Discord.ApiClient.ModifyMessage(guildChannel.Guild.Id, Channel.Id, Id, args).ConfigureAwait(false);
            else
                await Discord.ApiClient.ModifyDMMessage(Channel.Id, Id, args).ConfigureAwait(false);
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
        IReadOnlyList<ulong> IMessage.MentionedChannelIds => MentionedChannels.Select(x => x.Id).ToImmutableArray();
        IReadOnlyList<ulong> IMessage.MentionedRoleIds => MentionedRoles.Select(x => x.Id).ToImmutableArray();
    }
}
