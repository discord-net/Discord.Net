using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestTextChannel : RestGuildChannel, IRestMessageChannel, ITextChannel
    {
        public string Topic { get; private set; }

        public string Mention => MentionUtils.MentionChannel(Id);

        internal RestTextChannel(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, guild, id)
        {
        }
        internal new static RestTextChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestTextChannel(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            base.Update(model);

            Topic = model.Topic.Value;
        }


        public Task ModifyAsync(Action<ModifyTextChannelParams> func)
            => ChannelHelper.ModifyAsync(this, Discord, func);

        public Task<RestGuildUser> GetUserAsync(ulong id)
            => ChannelHelper.GetUserAsync(this, Guild, Discord, id);
        public IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync()
            => ChannelHelper.GetUsersAsync(this, Guild, Discord);

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

        private string DebuggerDisplay => $"{Name} ({Id}, Text)";

        //IGuildChannel
        async Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetUserAsync(id);
            else
                return null;
        }
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode)
        {
            if (mode == CacheMode.AllowDownload)
                return GetUsersAsync();
            else
                return AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>(); //Overriden
        }

        //IMessageChannel
        async Task<IMessage> IMessageChannel.GetMessageAsync(ulong id, CacheMode mode)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetMessageAsync(id);
            else
                return null;
        }
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit, CacheMode mode)
        {
            if (mode == CacheMode.AllowDownload)
                return GetMessagesAsync(limit);
            else
                return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
        }
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(ulong fromMessageId, Direction dir, int limit, CacheMode mode)
        {
            if (mode == CacheMode.AllowDownload)
                return GetMessagesAsync(fromMessageId, dir, limit);
            else
                return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
        }
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage fromMessage, Direction dir, int limit, CacheMode mode)
        {
            if (mode == CacheMode.AllowDownload)
                return GetMessagesAsync(fromMessage, dir, limit);
            else
                return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
        }
        async Task<IReadOnlyCollection<IMessage>> IMessageChannel.GetPinnedMessagesAsync() 
            => await GetPinnedMessagesAsync();

        async Task<IUserMessage> IMessageChannel.SendFileAsync(string filePath, string text, bool isTTS)
            => await SendFileAsync(filePath, text, isTTS);
        async Task<IUserMessage> IMessageChannel.SendFileAsync(Stream stream, string filename, string text, bool isTTS) 
            => await SendFileAsync(stream, filename, text, isTTS);
        async Task<IUserMessage> IMessageChannel.SendMessageAsync(string text, bool isTTS) 
            => await SendMessageAsync(text, isTTS);
        IDisposable IMessageChannel.EnterTypingState() 
            => EnterTypingState();
    }
}
