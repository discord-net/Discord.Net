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
    internal class UserMessage : Message, IUserMessage
    {
        private bool _isMentioningEveryone, _isTTS, _isPinned;
        private long? _editedTimestampTicks;
        private IReadOnlyCollection<IAttachment> _attachments;
        private IReadOnlyCollection<IEmbed> _embeds;
        private IReadOnlyCollection<ulong> _mentionedChannelIds;
        private IReadOnlyCollection<IRole> _mentionedRoles;
        private IReadOnlyCollection<IUser> _mentionedUsers;

        public override DiscordRestClient Discord => (Channel as Entity<ulong>).Discord;
        public override bool IsTTS => _isTTS;
        public override bool IsPinned => _isPinned;
        public override DateTimeOffset? EditedTimestamp => DateTimeUtils.FromTicks(_editedTimestampTicks);

        public override IReadOnlyCollection<IAttachment> Attachments => _attachments;
        public override IReadOnlyCollection<IEmbed> Embeds => _embeds;
        public override IReadOnlyCollection<ulong> MentionedChannelIds => _mentionedChannelIds;
        public override IReadOnlyCollection<IRole> MentionedRoles => _mentionedRoles;
        public override IReadOnlyCollection<IUser> MentionedUsers => _mentionedUsers;

        public UserMessage(IMessageChannel channel, IUser author, Model model)
            : base(channel, author, model)
        {
            _mentionedChannelIds = ImmutableArray.Create<ulong>();
            _mentionedRoles = ImmutableArray.Create<IRole>();
            _mentionedUsers = ImmutableArray.Create<IUser>();

            Update(model, UpdateSource.Creation);
        }
        public override void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            var guildChannel = Channel as GuildChannel;
            var guild = guildChannel?.Guild;

            if (model.IsTextToSpeech.IsSpecified)
                _isTTS = model.IsTextToSpeech.Value;
            if (model.Pinned.IsSpecified)
                _isPinned = model.Pinned.Value;
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
                    _attachments = ImmutableArray.Create(attachments);
                }
                else
                    _attachments = ImmutableArray.Create<Attachment>();
            }

            if (model.Embeds.IsSpecified)
            {
                var value = model.Embeds.Value;
                if (value.Length > 0)
                {
                    var embeds = new Embed[value.Length];
                    for (int i = 0; i < embeds.Length; i++)
                        embeds[i] = new Embed(value[i]);
                    _embeds = ImmutableArray.Create(embeds);
                }
                else
                    _embeds = ImmutableArray.Create<Embed>();
            }

            ImmutableArray<IUser> mentions = ImmutableArray.Create<IUser>();
            if (model.Mentions.IsSpecified)
            {
                var value = model.Mentions.Value;
                if (value.Length > 0)
                {
                    var newMentions = new IUser[value.Length];
                    for (int i = 0; i < value.Length; i++)
                        newMentions[i] = new User(value[i]);
                    mentions = ImmutableArray.Create(newMentions);
                }
            }

            if (model.Content.IsSpecified)
            {
                var text = model.Content.Value;
                
                if (guildChannel != null)
                {
                    _mentionedUsers = MentionUtils.GetUserMentions(text, Channel, mentions);
                    _mentionedChannelIds = MentionUtils.GetChannelMentions(text, guildChannel.Guild);
                    _mentionedRoles = MentionUtils.GetRoleMentions(text, guildChannel.Guild);
                }
                model.Content = text;
            }

            base.Update(model, source);
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
        public async Task PinAsync()
        {
            await Discord.ApiClient.AddPinAsync(Channel.Id, Id).ConfigureAwait(false);
        }
        public async Task UnpinAsync()
        {
            await Discord.ApiClient.RemovePinAsync(Channel.Id, Id).ConfigureAwait(false);
        }
        
        public string Resolve(int startIndex, int length, UserMentionHandling userHandling, ChannelMentionHandling channelHandling,
            RoleMentionHandling roleHandling, EveryoneMentionHandling everyoneHandling)
            => Resolve(Content.Substring(startIndex, length), userHandling, channelHandling, roleHandling, everyoneHandling);
        public string Resolve(UserMentionHandling userHandling, ChannelMentionHandling channelHandling, 
            RoleMentionHandling roleHandling, EveryoneMentionHandling everyoneHandling)
            => Resolve(Content, userHandling, channelHandling, roleHandling, everyoneHandling);
        
        private string Resolve(string text, UserMentionHandling userHandling, ChannelMentionHandling channelHandling,
            RoleMentionHandling roleHandling, EveryoneMentionHandling everyoneHandling)
        {
            text = MentionUtils.ResolveUserMentions(text, Channel, MentionedUsers, userHandling);
            text = MentionUtils.ResolveChannelMentions(text, (Channel as IGuildChannel)?.Guild, channelHandling);
            text = MentionUtils.ResolveRoleMentions(text, MentionedRoles, roleHandling);
            text = MentionUtils.ResolveEveryoneMentions(text, everyoneHandling);
            return text;
        }

        public override string ToString() => Content;
        private string DebuggerDisplay => $"{Author}: {Content}{(Attachments.Count > 0 ? $" [{Attachments.Count} Attachments]" : "")}";
    }
}
