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
        public override void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            Topic = model.Topic;
            base.Update(model, source);
        }

        public async Task ModifyAsync(Action<ModifyTextChannelParams> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var args = new ModifyTextChannelParams();
            func(args);
            var model = await Discord.ApiClient.ModifyGuildChannelAsync(Id, args).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        
        public override async Task<IGuildUser> GetUserAsync(ulong id)
        {
            var user = await Guild.GetUserAsync(id).ConfigureAwait(false);
            if (user != null && Permissions.GetValue(Permissions.ResolveChannel(user, this, user.GuildPermissions.RawValue), ChannelPermission.ReadMessages))
                return user;
            return null;
        }
        public override async Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync()
        {
            var users = await Guild.GetUsersAsync().ConfigureAwait(false);
            return users.Where(x => Permissions.GetValue(Permissions.ResolveChannel(x, this, x.GuildPermissions.RawValue), ChannelPermission.ReadMessages)).ToImmutableArray();
        }
        public override async Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync(int limit, int offset)
        {
            var users = await Guild.GetUsersAsync(limit, offset).ConfigureAwait(false);
            return users.Where(x => Permissions.GetValue(Permissions.ResolveChannel(x, this, x.GuildPermissions.RawValue), ChannelPermission.ReadMessages)).ToImmutableArray();
        }

        public async Task<IMessage> SendMessageAsync(string text, bool isTTS)
        {
            var args = new CreateMessageParams { Content = text, IsTTS = isTTS };
            var model = await Discord.ApiClient.CreateMessageAsync(Guild.Id, Id, args).ConfigureAwait(false);
            return new Message(this, new User(Discord, model.Author), model);
        }
        public async Task<IMessage> SendFileAsync(string filePath, string text, bool isTTS)
        {
            string filename = Path.GetFileName(filePath);
            using (var file = File.OpenRead(filePath))
            {
                var args = new UploadFileParams { Filename = filename, Content = text, IsTTS = isTTS };
                var model = await Discord.ApiClient.UploadFileAsync(Guild.Id, Id, file, args).ConfigureAwait(false);
                return new Message(this, new User(Discord, model.Author), model);
            }
        }
        public async Task<IMessage> SendFileAsync(Stream stream, string filename, string text, bool isTTS)
        {
            var args = new UploadFileParams { Filename = filename, Content = text, IsTTS = isTTS };
            var model = await Discord.ApiClient.UploadFileAsync(Guild.Id, Id, stream, args).ConfigureAwait(false);
            return new Message(this, new User(Discord, model.Author), model);
        }
        public virtual async Task<IMessage> GetMessageAsync(ulong id)
        {
            var model = await Discord.ApiClient.GetChannelMessageAsync(Id, id).ConfigureAwait(false);
            if (model != null)
                return new Message(this, new User(Discord, model.Author), model);
            return null;
        }
        public virtual async Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit)
        {
            var args = new GetChannelMessagesParams { Limit = limit };
            var models = await Discord.ApiClient.GetChannelMessagesAsync(Id, args).ConfigureAwait(false);
            return models.Select(x => new Message(this, new User(Discord, x.Author), x)).ToImmutableArray();
        }
        public virtual async Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit)
        {
            var args = new GetChannelMessagesParams { Limit = limit };
            var models = await Discord.ApiClient.GetChannelMessagesAsync(Id, args).ConfigureAwait(false);
            return models.Select(x => new Message(this, new User(Discord, x.Author), x)).ToImmutableArray();
        }
        public async Task DeleteMessagesAsync(IEnumerable<IMessage> messages)
        {
            await Discord.ApiClient.DeleteMessagesAsync(Guild.Id, Id, new DeleteMessagesParams { MessageIds = messages.Select(x => x.Id) }).ConfigureAwait(false);
        }
        
        public async Task TriggerTypingAsync()
        {
            await Discord.ApiClient.TriggerTypingIndicatorAsync(Id).ConfigureAwait(false);
        }

        private string DebuggerDisplay => $"{Name} ({Id}, Text)";

        IMessage IMessageChannel.GetCachedMessage(ulong id) => null;
    }
}
