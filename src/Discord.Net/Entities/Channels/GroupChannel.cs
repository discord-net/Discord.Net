using Discord.API.Rest;
using Discord.Extensions;
using System;
using System.Collections.Concurrent;
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
    internal class GroupChannel : SnowflakeEntity, IGroupChannel
    {
        protected ConcurrentDictionary<ulong, GroupUser> _users;
        private string _iconId;

        public override DiscordRestClient Discord { get; }
        public string Name { get; private set; }

        public IReadOnlyCollection<IUser> Recipients => _users.ToReadOnlyCollection();
        public virtual IReadOnlyCollection<IMessage> CachedMessages => ImmutableArray.Create<IMessage>();
        public string IconUrl => API.CDN.GetChannelIconUrl(Id, _iconId);

        public GroupChannel(DiscordRestClient discord, Model model)
            : base(model.Id)
        {
            Discord = discord;

            Update(model, UpdateSource.Creation);
        }
        public virtual void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            if (model.Name.IsSpecified)
                Name = model.Name.Value;
            if (model.Icon.IsSpecified)
                _iconId = model.Icon.Value;

            if (source != UpdateSource.Creation && model.Recipients.IsSpecified)
                UpdateUsers(model.Recipients.Value, source);
        }

        internal virtual void UpdateUsers(API.User[] models, UpdateSource source)
        {
            if (!IsAttached)
            {
                var users = new ConcurrentDictionary<ulong, GroupUser>(1, (int)(models.Length * 1.05));
                for (int i = 0; i < models.Length; i++)
                    users[models[i].Id] = new GroupUser(this, new User(models[i]));
                _users = users;
            }
        }

        public async Task UpdateAsync()
        {
            if (IsAttached) throw new NotSupportedException();

            var model = await Discord.ApiClient.GetChannelAsync(Id).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task LeaveAsync()
        {
            await Discord.ApiClient.DeleteChannelAsync(Id).ConfigureAwait(false);
        }

        public async Task AddUserAsync(IUser user)
        {
            await Discord.ApiClient.AddGroupRecipientAsync(Id, user.Id).ConfigureAwait(false);
        }
        public async Task<IUser> GetUserAsync(ulong id)
        {
            GroupUser user;
            if (_users.TryGetValue(id, out user))
                return user;
            var currentUser = await Discord.GetCurrentUserAsync().ConfigureAwait(false);
            if (id == currentUser.Id)
                return currentUser;
            return null;
        }
        public async Task<IReadOnlyCollection<IUser>> GetUsersAsync()
        {
            var currentUser = await Discord.GetCurrentUserAsync().ConfigureAwait(false);
            return _users.Select(x => x.Value).Concat<IUser>(ImmutableArray.Create(currentUser)).ToReadOnlyCollection(_users);
        }

        public async Task<IMessage> SendMessageAsync(string text, bool isTTS)
        {
            var args = new CreateMessageParams { Content = text, IsTTS = isTTS };
            var model = await Discord.ApiClient.CreateDMMessageAsync(Id, args).ConfigureAwait(false);
            return new Message(this, new User(model.Author.Value), model);
        }
        public async Task<IMessage> SendFileAsync(string filePath, string text, bool isTTS)
        {
            string filename = Path.GetFileName(filePath);
            using (var file = File.OpenRead(filePath))
            {
                var args = new UploadFileParams(file) { Filename = filename, Content = text, IsTTS = isTTS };
                var model = await Discord.ApiClient.UploadDMFileAsync(Id, args).ConfigureAwait(false);
                return new Message(this, new User(model.Author.Value), model);
            }
        }
        public async Task<IMessage> SendFileAsync(Stream stream, string filename, string text, bool isTTS)
        {
            var args = new UploadFileParams(stream) { Filename = filename, Content = text, IsTTS = isTTS };
            var model = await Discord.ApiClient.UploadDMFileAsync(Id, args).ConfigureAwait(false);
            return new Message(this, new User(model.Author.Value), model);
        }
        public virtual async Task<IMessage> GetMessageAsync(ulong id)
        {
            var model = await Discord.ApiClient.GetChannelMessageAsync(Id, id).ConfigureAwait(false);
            if (model != null)
                return new Message(this, new User(model.Author.Value), model);
            return null;
        }
        public virtual async Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit)
        {
            var args = new GetChannelMessagesParams { Limit = limit };
            var models = await Discord.ApiClient.GetChannelMessagesAsync(Id, args).ConfigureAwait(false);
            return models.Select(x => new Message(this, new User(x.Author.Value), x)).ToImmutableArray();
        }
        public virtual async Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit)
        {
            var args = new GetChannelMessagesParams { Limit = limit };
            var models = await Discord.ApiClient.GetChannelMessagesAsync(Id, args).ConfigureAwait(false);
            return models.Select(x => new Message(this, new User(x.Author.Value), x)).ToImmutableArray();
        }
        public async Task DeleteMessagesAsync(IEnumerable<IMessage> messages)
        {
            await Discord.ApiClient.DeleteDMMessagesAsync(Id, new DeleteMessagesParams { MessageIds = messages.Select(x => x.Id) }).ConfigureAwait(false);
        }

        public async Task TriggerTypingAsync()
        {
            await Discord.ApiClient.TriggerTypingIndicatorAsync(Id).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"@{Name} ({Id}, Group)";

        IMessage IMessageChannel.GetCachedMessage(ulong id) => null;
    }
}
