using Discord.API.Rest;
using Discord.Net;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord
{
    public class Message : IEntity<ulong>
    {
        //TODO: Docstrings

        /// <inheritdoc />
        public ulong Id { get; }
        public IMessageChannel Channel { get; }
        public IUser User { get; }

        public bool IsTTS { get; private set; }
        public string RawText { get; private set; }
        public string Text { get; private set; }
        public DateTime Timestamp { get; private set; }
        public DateTime? EditedTimestamp { get; private set; }
        public IReadOnlyList<Attachment> Attachments { get; private set; }
        public IReadOnlyList<Embed> Embeds { get; private set; }
        public IReadOnlyList<GuildUser> MentionedUsers { get; private set; }
        public IReadOnlyList<GuildChannel> MentionedChannels { get; private set; }
        public IReadOnlyList<Role> MentionedRoles { get; private set; }
        
        public DiscordClient Discord => Channel.Discord;
        public bool IsAuthor => User.Id == Discord.CurrentUser.Id;

        internal Message(ulong id, IMessageChannel channel, IUser user)
        {
            Id = id;
            Channel = channel;
            User = user;
        }

        internal void Update(Model model)
        {
            var channel = Channel;
            bool isPublic = channel.Type != ChannelType.DM;

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
                var users = new GuildUser[model.Mentions.Length];
                int j = 0;
                for (int i = 0; i < users.Length; i++)
                {
                    var user = Channel.GetUser(model.Mentions[i].Id) as GuildUser;
                    if (user != null)
                        users[j++] = user;
                }
                MentionedUsers = ImmutableArray.Create(users, 0, j);
            }
            else
                MentionedUsers = ImmutableArray<GuildUser>.Empty;

            if (model.IsMentioningEveryone && isPublic)
                MentionedRoles = ImmutableArray.Create((channel as GuildChannel).Guild.EveryoneRole);
            else
                MentionedRoles = ImmutableArray<Role>.Empty;

            string text = model.Content;                
            if (isPublic)
            {
                var publicChannel = channel as GuildChannel;
                var mentionedChannels = ImmutableArray.CreateBuilder<GuildChannel>();
                text = MentionHelper.CleanUserMentions(publicChannel, text);
                text = MentionHelper.CleanChannelMentions(publicChannel, text, mentionedChannels);
                MentionedChannels = mentionedChannels.ToImmutable();
            }
            else
                MentionedChannels = ImmutableArray<GuildChannel>.Empty;
            Text = text;                
        }

        /// <summary> Returns true if the logged-in user was mentioned. </summary>
        public bool IsMentioningMe(bool includeRoles = false)
        {
            var me = Channel.GetCurrentUser() as GuildUser;
            if (me != null)
            {
                if (includeRoles)
                    return MentionedUsers.Contains(me) || MentionedRoles.Any(x => me.HasRole(x));
                else
                    return MentionedUsers.Contains(me);
            }
            return false;
        }

        public async Task Modify(Action<ModifyMessageRequest> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var req = new ModifyMessageRequest(Channel.Id, Id);
            func(req);
            await Discord.RestClient.Send(req).ConfigureAwait(false);
        }

        public Task Update() { throw new NotSupportedException(); } //TODO: Not supported yet 

        /// <summary> Deletes this message. </summary>
        public async Task Delete()
        {
            try { await Discord.RestClient.Send(new DeleteMessageRequest(Channel.Id, Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }
    }
}
