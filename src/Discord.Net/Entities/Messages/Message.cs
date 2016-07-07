using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal class Message : SnowflakeEntity, IMessage
    {
        private bool _isMentioningEveryone;
        private long _timestampTicks;
        private long? _editedTimestampTicks;

        public bool IsTTS { get; private set; }
        public string RawText { get; private set; }
        public string Text { get; private set; }
        public bool IsPinned { get; private set; }
        
        public IMessageChannel Channel { get; }
        public IUser Author { get; }
        
        public IReadOnlyCollection<IAttachment> Attachments { get; private set; }
        public IReadOnlyCollection<IEmbed> Embeds { get; private set; }
        public IReadOnlyCollection<ulong> MentionedChannelIds { get; private set; }
        public IReadOnlyCollection<IRole> MentionedRoles { get; private set; }
        public IReadOnlyCollection<IUser> MentionedUsers { get; private set; }

        public override DiscordClient Discord => (Channel as Entity<ulong>).Discord;
        public DateTimeOffset? EditedTimestamp => DateTimeUtils.FromTicks(_editedTimestampTicks);
        public DateTimeOffset Timestamp => DateTimeUtils.FromTicks(_timestampTicks);

        public Message(IMessageChannel channel, IUser author, Model model)
            : base(model.Id)
        {
            Channel = channel;
            Author = author;

            if (channel is IGuildChannel)
            {
                MentionedUsers = ImmutableArray.Create<IUser>();
                MentionedChannelIds = ImmutableArray.Create<ulong>();
                MentionedRoles = ImmutableArray.Create<IRole>();
            }

            Update(model, UpdateSource.Creation);
        }
        public void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            var guildChannel = Channel as GuildChannel;
            var guild = guildChannel?.Guild;
            var discord = Discord;

            if (model.IsTextToSpeech.IsSpecified)
                IsTTS = model.IsTextToSpeech.Value;
            if (model.Pinned.IsSpecified)
                IsPinned = model.Pinned.Value;
            if (model.Timestamp.IsSpecified)
                _timestampTicks = model.Timestamp.Value.UtcTicks;
            if (model.EditedTimestamp.IsSpecified)
                _editedTimestampTicks = model.EditedTimestamp.Value?.UtcTicks;
            if (model.MentionEveryone.IsSpecified)
                _isMentioningEveryone = model.MentionEveryone.Value;

            if (model.Attachments.IsSpecified)
            {
                var value = model.Attachments.Value;
                if (value.Length > 0)
                {
                    var attachments = new Attachment[value.Length];
                    for (int i = 0; i < attachments.Length; i++)
                        attachments[i] = new Attachment(value[i]);
                    Attachments = ImmutableArray.Create(attachments);
                }
                else
                    Attachments = ImmutableArray.Create<Attachment>();
            }

            if (model.Embeds.IsSpecified)
            {
                var value = model.Embeds.Value;
                if (value.Length > 0)
                {
                    var embeds = new Embed[value.Length];
                    for (int i = 0; i < embeds.Length; i++)
                        embeds[i] = new Embed(value[i]);
                    Embeds = ImmutableArray.Create(embeds);
                }
                else
                    Embeds = ImmutableArray.Create<Embed>();
            }

            if (model.Mentions.IsSpecified)
            {
                var value = model.Mentions.Value;
                if (value.Length > 0)
                {
                    var mentions = new User[value.Length];
                    for (int i = 0; i < value.Length; i++)
                        mentions[i] = new User(value[i]);
                    MentionedUsers = ImmutableArray.Create(mentions);
                }
                else
                    MentionedUsers = ImmutableArray.Create<IUser>();
            }

            if (model.Content.IsSpecified)
            {
                RawText = model.Content.Value;
                
                if (guildChannel != null)
                {
                    var orderedMentionedUsers = ImmutableArray.CreateBuilder<IUser>(5);
                    Text = MentionUtils.CleanUserMentions(RawText, Channel.IsAttached ? Channel : null, MentionedUsers, orderedMentionedUsers);
                    MentionedUsers = orderedMentionedUsers.ToImmutable();

                    var roles = ImmutableArray.CreateBuilder<IRole>(5);
                    Text = MentionUtils.CleanRoleMentions(Text, guildChannel.Guild, roles);
                    MentionedRoles = roles.ToImmutable();

                    if (guildChannel.IsAttached) //It's too expensive to do a channel lookup in REST mode
                    {
                        var channelIds = ImmutableArray.CreateBuilder<ulong>(5);
                        Text = MentionUtils.CleanChannelMentions(Text, guildChannel.Guild, channelIds);
                        MentionedChannelIds = channelIds.ToImmutable();
                    }
                    else
                        MentionedChannelIds = MentionUtils.GetChannelMentions(RawText);
                }
                else
                    Text = RawText;
            }
        }

        public async Task UpdateAsync()
        {
            if (IsAttached) throw new NotSupportedException();

            var model = await Discord.ApiClient.GetChannelMessageAsync(Channel.Id, Id).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task ModifyAsync(Action<ModifyMessageParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyMessageParams();
            func(args);
            var guildChannel = Channel as GuildChannel;

            Model model;
            if (guildChannel != null)
                model = await Discord.ApiClient.ModifyMessageAsync(guildChannel.Guild.Id, Channel.Id, Id, args).ConfigureAwait(false);
            else
                model = await Discord.ApiClient.ModifyDMMessageAsync(Channel.Id, Id, args).ConfigureAwait(false);
        {
            await Discord.ApiClient.AddPinAsync(Channel.Id, Id).ConfigureAwait(false);
        }
            Update(model, UpdateSource.Rest);
        }        
        public async Task DeleteAsync()
        {
            var guildChannel = Channel as GuildChannel;
            if (guildChannel != null)
                await Discord.ApiClient.DeleteMessageAsync(guildChannel.Id, Channel.Id, Id).ConfigureAwait(false);
            else
                await Discord.ApiClient.DeleteDMMessageAsync(Channel.Id, Id).ConfigureAwait(false);
        }
        /// <summary> Adds this message to its channel's pinned messages. </summary>
        public async Task PinAsync()
        {
            await Discord.ApiClient.AddPinAsync(Channel.Id, Id).ConfigureAwait(false);
        }
        /// <summary> Removes this message from its channel's pinned messages. </summary>
        public async Task UnpinAsync()
        {
            await Discord.ApiClient.RemovePinAsync(Channel.Id, Id).ConfigureAwait(false);
        }

        public override string ToString() => Text;
        private string DebuggerDisplay => $"{Author}: {Text}{(Attachments.Count > 0 ? $" [{Attachments.Count} Attachments]" : "")}";
    }
}
