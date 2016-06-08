using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal class TextChannel : GuildChannel, ITextChannel
    {
        public string Topic { get; private set; }
        
        public string Mention => MentionUtils.Mention(this);
        public virtual IReadOnlyCollection<IMessage> CachedMessages => ImmutableArray.Create<IMessage>();

        public TextChannel(Guild guild, Model model)
            : base(guild, model)
        {
        }
        protected override void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            Topic = model.Topic;
            base.Update(model, UpdateSource.Rest);
        }

        public async Task Modify(Action<ModifyTextChannelParams> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var args = new ModifyTextChannelParams();
            func(args);
            var model = await Discord.ApiClient.ModifyGuildChannel(Id, args).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        
        public override async Task<IGuildUser> GetUser(ulong id)
        {
            var user = await Guild.GetUser(id).ConfigureAwait(false);
            if (user != null && Permissions.GetValue(Permissions.ResolveChannel(user, this, user.GuildPermissions.RawValue), ChannelPermission.ReadMessages))
                return user;
            return null;
        }
        public override async Task<IReadOnlyCollection<IGuildUser>> GetUsers()
        {
            var users = await Guild.GetUsers().ConfigureAwait(false);
            return users.Where(x => Permissions.GetValue(Permissions.ResolveChannel(x, this, x.GuildPermissions.RawValue), ChannelPermission.ReadMessages)).ToImmutableArray();
        }
        public override async Task<IReadOnlyCollection<IGuildUser>> GetUsers(int limit, int offset)
        {
            var users = await Guild.GetUsers(limit, offset).ConfigureAwait(false);
            return users.Where(x => Permissions.GetValue(Permissions.ResolveChannel(x, this, x.GuildPermissions.RawValue), ChannelPermission.ReadMessages)).ToImmutableArray();
        }

        public async Task<IMessage> SendMessage(string text, bool isTTS)
        {
            var args = new CreateMessageParams { Content = text, IsTTS = isTTS };
            var model = await Discord.ApiClient.CreateMessage(Guild.Id, Id, args).ConfigureAwait(false);
            return new Message(this, new User(Discord, model.Author), model);
        }
        public async Task<IMessage> SendFile(string filePath, string text, bool isTTS)
        {
            string filename = Path.GetFileName(filePath);
            using (var file = File.OpenRead(filePath))
            {
                var args = new UploadFileParams { Filename = filename, Content = text, IsTTS = isTTS };
                var model = await Discord.ApiClient.UploadFile(Guild.Id, Id, file, args).ConfigureAwait(false);
                return new Message(this, new User(Discord, model.Author), model);
            }
        }
        public async Task<IMessage> SendFile(Stream stream, string filename, string text, bool isTTS)
        {
            var args = new UploadFileParams { Filename = filename, Content = text, IsTTS = isTTS };
            var model = await Discord.ApiClient.UploadFile(Guild.Id, Id, stream, args).ConfigureAwait(false);
            return new Message(this, new User(Discord, model.Author), model);
        }
        public virtual async Task<IMessage> GetMessage(ulong id)
        {
            var model = await Discord.ApiClient.GetChannelMessage(Id, id).ConfigureAwait(false);
            if (model != null)
                return new Message(this, new User(Discord, model.Author), model);
            return null;
        }
        public virtual async Task<IReadOnlyCollection<IMessage>> GetMessages(int limit)
        {
            var args = new GetChannelMessagesParams { Limit = limit };
            var models = await Discord.ApiClient.GetChannelMessages(Id, args).ConfigureAwait(false);
            return models.Select(x => new Message(this, new User(Discord, x.Author), x)).ToImmutableArray();
        }
        public virtual async Task<IReadOnlyCollection<IMessage>> GetMessages(ulong fromMessageId, Direction dir, int limit)
        {
            var args = new GetChannelMessagesParams { Limit = limit };
            var models = await Discord.ApiClient.GetChannelMessages(Id, args).ConfigureAwait(false);
            return models.Select(x => new Message(this, new User(Discord, x.Author), x)).ToImmutableArray();
        }
        public async Task DeleteMessages(IEnumerable<IMessage> messages)
        {
            await Discord.ApiClient.DeleteMessages(Guild.Id, Id, new DeleteMessagesParams { MessageIds = messages.Select(x => x.Id) }).ConfigureAwait(false);
        }
        
        public async Task TriggerTyping()
        {
            await Discord.ApiClient.TriggerTypingIndicator(Id).ConfigureAwait(false);
        }

        private string DebuggerDisplay => $"{Name} ({Id}, Text)";

        IMessage IMessageChannel.GetCachedMessage(ulong id) => null;
    }
}
