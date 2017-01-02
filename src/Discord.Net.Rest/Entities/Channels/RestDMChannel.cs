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
    public class RestDMChannel : RestChannel, IDMChannel, IRestPrivateChannel, IRestMessageChannel, IUpdateable
    {
        public RestUser CurrentUser { get; private set; }
        public RestUser Recipient { get; private set; }

        public IReadOnlyCollection<RestUser> Users => ImmutableArray.Create(CurrentUser, Recipient);

        internal RestDMChannel(BaseDiscordClient discord, ulong id, ulong recipientId)
            : base(discord, id)
        {
            Recipient = new RestUser(Discord, recipientId);
            CurrentUser = new RestUser(Discord, discord.CurrentUser.Id);
        }
        internal new static RestDMChannel Create(BaseDiscordClient discord, Model model)
        {
            var entity = new RestDMChannel(discord, model.Id, model.Recipients.Value[0].Id);
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            Recipient.Update(model.Recipients.Value[0]);
        }

        public override async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetChannelAsync(Id, options).ConfigureAwait(false);
            Update(model); 
        }
        public Task CloseAsync(RequestOptions options = null)
            => ChannelHelper.DeleteAsync(this, Discord, options);

        public RestUser GetUser(ulong id)
        {
            if (id == Recipient.Id)
                return Recipient;
            else if (id == Discord.CurrentUser.Id)
                return CurrentUser;
            else
                return null;
        }

        public Task<RestMessage> GetMessageAsync(ulong id, RequestOptions options = null)
            => ChannelHelper.GetMessageAsync(this, Discord, id, options);
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => ChannelHelper.GetMessagesAsync(this, Discord, null, Direction.Before, limit, options);
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => ChannelHelper.GetMessagesAsync(this, Discord, fromMessageId, dir, limit, options);
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => ChannelHelper.GetMessagesAsync(this, Discord, fromMessage.Id, dir, limit, options);
        public Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions options = null)
            => ChannelHelper.GetPinnedMessagesAsync(this, Discord, options);

        public Task<RestUserMessage> SendMessageAsync(string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ChannelHelper.SendMessageAsync(this, Discord, text, isTTS, embed, options);
#if NETSTANDARD1_3
        public Task<RestUserMessage> SendFileAsync(string filePath, string text, bool isTTS = false, RequestOptions options = null)
            => ChannelHelper.SendFileAsync(this, Discord, filePath, text, isTTS, options);
#endif
        public Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text, bool isTTS = false, RequestOptions options = null)
            => ChannelHelper.SendFileAsync(this, Discord, stream, filename, text, isTTS, options);

        public Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions options = null)
            => ChannelHelper.DeleteMessagesAsync(this, Discord, messages, options);

        public Task TriggerTypingAsync(RequestOptions options = null)
            => ChannelHelper.TriggerTypingAsync(this, Discord, options);
        public IDisposable EnterTypingState(RequestOptions options = null)
            => ChannelHelper.EnterTypingState(this, Discord, options);

        public override string ToString() => $"@{Recipient}";
        private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";

        //IDMChannel            
        IUser IDMChannel.Recipient => Recipient;

        //IRestPrivateChannel
        IReadOnlyCollection<RestUser> IRestPrivateChannel.Recipients => ImmutableArray.Create(Recipient);

        //IPrivateChannel
        IReadOnlyCollection<IUser> IPrivateChannel.Recipients => ImmutableArray.Create<IUser>(Recipient);

        //IMessageChannel
        async Task<IMessage> IMessageChannel.GetMessageAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetMessageAsync(id, options).ConfigureAwait(false);
            else
                return null;
        }
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return GetMessagesAsync(limit, options);
            else
                return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
        }
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(ulong fromMessageId, Direction dir, int limit, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return GetMessagesAsync(fromMessageId, dir, limit, options);
            else
                return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
        }
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage fromMessage, Direction dir, int limit, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return GetMessagesAsync(fromMessage, dir, limit, options);
            else
                return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
        }
        async Task<IReadOnlyCollection<IMessage>> IMessageChannel.GetPinnedMessagesAsync(RequestOptions options)
            => await GetPinnedMessagesAsync(options).ConfigureAwait(false);

#if NETSTANDARD1_3
        async Task<IUserMessage> IMessageChannel.SendFileAsync(string filePath, string text, bool isTTS, RequestOptions options)
            => await SendFileAsync(filePath, text, isTTS, options).ConfigureAwait(false);
#endif
        async Task<IUserMessage> IMessageChannel.SendFileAsync(Stream stream, string filename, string text, bool isTTS, RequestOptions options)
            => await SendFileAsync(stream, filename, text, isTTS, options).ConfigureAwait(false);
        async Task<IUserMessage> IMessageChannel.SendMessageAsync(string text, bool isTTS, Embed embed, RequestOptions options)
            => await SendMessageAsync(text, isTTS, embed, options).ConfigureAwait(false);
        IDisposable IMessageChannel.EnterTypingState(RequestOptions options)
            => EnterTypingState(options);

        //IChannel
        string IChannel.Name => $"@{Recipient}";

        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(GetUser(id));
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable();
    }
}
