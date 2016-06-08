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

            Update(model, UpdateSource.Creation);
        }
        public void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

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
                Attachments = ImmutableArray.Create<Attachment>();

            if (model.Embeds.Length > 0)
            {
                var embeds = new Embed[model.Attachments.Length];
                for (int i = 0; i < embeds.Length; i++)
                    embeds[i] = new Embed(model.Embeds[i]);
                Embeds = ImmutableArray.Create(embeds);
            }
            else
                Embeds = ImmutableArray.Create<Embed>();

            if (guildChannel != null && model.Mentions.Length > 0)
            {
                var mentions = new User[model.Mentions.Length];
                for (int i = 0; i < model.Mentions.Length; i++)
                    mentions[i] = new User(discord, model.Mentions[i]);
                MentionedUsers = ImmutableArray.Create(mentions);
            }
            else
                MentionedUsers = ImmutableArray.Create<User>();

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
                MentionedChannelIds = ImmutableArray.Create<ulong>();
                MentionedRoleIds = ImmutableArray.Create<ulong>();
            }
            
            Text = MentionUtils.CleanUserMentions(model.Content, model.Mentions);
        }

        public async Task Update()
        {
            if (IsAttached) throw new NotSupportedException();

            var model = await Discord.ApiClient.GetChannelMessage(Channel.Id, Id).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
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
            Update(model, UpdateSource.Rest);
        }        
        public async Task Delete()
        {
            var guildChannel = Channel as GuildChannel;
            if (guildChannel != null)
                await Discord.ApiClient.DeleteMessage(guildChannel.Id, Channel.Id, Id).ConfigureAwait(false);
            else
                await Discord.ApiClient.DeleteDMMessage(Channel.Id, Id).ConfigureAwait(false);
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
