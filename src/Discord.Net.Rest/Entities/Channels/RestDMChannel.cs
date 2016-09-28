using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestDMChannel : RestChannel, IDMChannel, IUpdateable
    {
        public RestUser CurrentUser { get; private set; }
        public RestUser Recipient { get; private set; }

        public IReadOnlyCollection<RestUser> Users => ImmutableArray.Create(CurrentUser, Recipient);

        internal RestDMChannel(DiscordClient discord, ulong id, ulong recipientId)
            : base(discord, id)
        {
            Recipient = new RestUser(Discord, recipientId);
            CurrentUser = new RestUser(Discord, discord.CurrentUser.Id);
        }
        internal new static RestDMChannel Create(DiscordClient discord, Model model)
        {
            var entity = new RestDMChannel(discord, model.Id, model.Recipients.Value[0].Id);
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            Recipient.Update(model.Recipients.Value[0]);
        }

        public override async Task UpdateAsync()
            => Update(await ChannelHelper.GetAsync(this, Discord));
        public Task CloseAsync()
            => ChannelHelper.DeleteAsync(this, Discord);

        public RestUser GetUser(ulong id)
        {
            if (id == Recipient.Id)
                return Recipient;
            else if (id == Discord.CurrentUser.Id)
                return CurrentUser;
            else
                return null;
        }

        public Task<RestMessage> GetMessageAsync(ulong id)
            => ChannelHelper.GetMessageAsync(this, Discord, id);
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch)
            => ChannelHelper.GetMessagesAsync(this, Discord, limit: limit);
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => ChannelHelper.GetMessagesAsync(this, Discord, fromMessageId, dir, limit);
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => ChannelHelper.GetMessagesAsync(this, Discord, fromMessage.Id, dir, limit);
        public Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync()
            => ChannelHelper.GetPinnedMessagesAsync(this, Discord);

        public Task<RestUserMessage> SendMessageAsync(string text, bool isTTS)
            => ChannelHelper.SendMessageAsync(this, Discord, text, isTTS);
        public Task<RestUserMessage> SendFileAsync(string filePath, string text, bool isTTS)
            => ChannelHelper.SendFileAsync(this, Discord, filePath, text, isTTS);
        public Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text, bool isTTS)
            => ChannelHelper.SendFileAsync(this, Discord, stream, filename, text, isTTS);

        public Task DeleteMessagesAsync(IEnumerable<IMessage> messages)
            => ChannelHelper.DeleteMessagesAsync(this, Discord, messages);

        public IDisposable EnterTypingState()
            => ChannelHelper.EnterTypingState(this, Discord);

        public override string ToString() => $"@{Recipient}";
        private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";
        
        //IDMChannel            
        IUser IDMChannel.Recipient => Recipient;

        //IPrivateChannel
        IReadOnlyCollection<IUser> IPrivateChannel.Recipients => ImmutableArray.Create<IUser>(Recipient);

        //IMessageChannel
        IReadOnlyCollection<IMessage> IMessageChannel.CachedMessages => ImmutableArray.Create<IMessage>();
        IMessage IMessageChannel.GetCachedMessage(ulong id) => null;

        async Task<IMessage> IMessageChannel.GetMessageAsync(ulong id)
            => await GetMessageAsync(id);
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit)
            => GetMessagesAsync(limit);
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(ulong fromMessageId, Direction dir, int limit)
            => GetMessagesAsync(fromMessageId, dir, limit);
        async Task<IReadOnlyCollection<IMessage>> IMessageChannel.GetPinnedMessagesAsync()
            => await GetPinnedMessagesAsync().ConfigureAwait(false);
        async Task<IUserMessage> IMessageChannel.SendFileAsync(string filePath, string text, bool isTTS)
            => await SendFileAsync(filePath, text, isTTS);
        async Task<IUserMessage> IMessageChannel.SendFileAsync(Stream stream, string filename, string text, bool isTTS)
            => await SendFileAsync(stream, filename, text, isTTS);
        async Task<IUserMessage> IMessageChannel.SendMessageAsync(string text, bool isTTS)
            => await SendMessageAsync(text, isTTS);
        IDisposable IMessageChannel.EnterTypingState()
            => EnterTypingState();

        //IChannel
        IReadOnlyCollection<IUser> IChannel.CachedUsers => Users;

        IUser IChannel.GetCachedUser(ulong id)
            => GetUser(id);
        Task<IUser> IChannel.GetUserAsync(ulong id)
            => Task.FromResult<IUser>(GetUser(id));
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync()
            => ImmutableArray.Create<IReadOnlyCollection<IUser>>().ToAsyncEnumerable();
    }
}
