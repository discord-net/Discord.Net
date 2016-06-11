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

        public DateTime? EditedTimestamp { get; private set; }
        public bool IsTTS { get; private set; }
        public string RawText { get; private set; }
        public string Text { get; private set; }
        public DateTime Timestamp { get; private set; }
        
        public IMessageChannel Channel { get; }
        public IUser Author { get; }
        
        public ImmutableArray<Attachment> Attachments { get; private set; }
        public ImmutableArray<Embed> Embeds { get; private set; }
        public ImmutableArray<ulong> MentionedChannelIds { get; private set; }
        public ImmutableArray<ulong> MentionedRoleIds { get; private set; }
        public ImmutableArray<User> MentionedUsers { get; private set; }

        public override DiscordClient Discord => (Channel as Entity<ulong>).Discord;

        public Message(IMessageChannel channel, IUser author, Model model)
            : base(model.Id)
        {
            Channel = channel;
            Author = author;

            if (channel is IGuildChannel)
            {
                MentionedUsers = ImmutableArray.Create<User>();
                MentionedChannelIds = ImmutableArray.Create<ulong>();
                MentionedRoleIds = ImmutableArray.Create<ulong>();
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
            if (model.Timestamp.IsSpecified)
                Timestamp = model.Timestamp.Value;
            if (model.EditedTimestamp.IsSpecified)
                EditedTimestamp = model.EditedTimestamp.Value;
            if (model.IsMentioningEveryone.IsSpecified)
                _isMentioningEveryone = model.IsMentioningEveryone.Value;
            
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
                        mentions[i] = new User(discord, value[i]);
                    MentionedUsers = ImmutableArray.Create(mentions);
                }
                else
                    MentionedUsers = ImmutableArray.Create<User>();
            }

            if (model.Content.IsSpecified)
            {
                RawText = model.Content.Value;

                if (Channel is IGuildChannel)
                {
                    Text = MentionUtils.CleanUserMentions(RawText, MentionedUsers);
                    MentionedChannelIds = MentionUtils.GetChannelMentions(RawText);
                    var mentionedRoleIds = MentionUtils.GetRoleMentions(RawText);
                    if (_isMentioningEveryone)
                        mentionedRoleIds = mentionedRoleIds.Add(guildChannel.Guild.EveryoneRole.Id);
                    MentionedRoleIds = mentionedRoleIds;
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

        public override string ToString() => Text;
        private string DebuggerDisplay => $"{Author}: {Text}{(Attachments.Length > 0 ? $" [{Attachments.Length} Attachments]" : "")}";

        IReadOnlyCollection<Attachment> IMessage.Attachments => Attachments;
        IReadOnlyCollection<IEmbed> IMessage.Embeds => Embeds;
        IReadOnlyCollection<ulong> IMessage.MentionedChannelIds => MentionedChannelIds;
        IReadOnlyCollection<ulong> IMessage.MentionedRoleIds => MentionedRoleIds;
        IReadOnlyCollection<IUser> IMessage.MentionedUsers => MentionedUsers;
    }
}
