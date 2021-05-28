using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using JetBrains.Annotations;

namespace Discord.Net.Examples.WebSocket
{
    [PublicAPI]
    internal class BaseSocketClientExamples
    {
        #region ReactionAdded

        public void HookReactionAdded(BaseSocketClient client)
            => client.ReactionAdded += HandleReactionAddedAsync;

        public async Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> cachedMessage,
            Cacheable<IMessageChannel, ulong> originChannel, SocketReaction reaction)
        {
            var message = await cachedMessage.GetOrDownloadAsync();
            if (message != null && reaction.User.IsSpecified)
                Console.WriteLine($"{reaction.User.Value} just added a reaction '{reaction.Emote}' " +
                                  $"to {message.Author}'s message ({message.Id}).");
        }

        #endregion

        #region ChannelCreated

        public void HookChannelCreated(BaseSocketClient client)
            => client.ChannelCreated += HandleChannelCreated;

        public Task HandleChannelCreated(SocketChannel channel)
        {
            if (channel is SocketGuildChannel guildChannel)
                Console.WriteLine($"A new channel '{guildChannel.Name}'({guildChannel.Id}, {guildChannel.GetType()})"
                                  + $"has been created at {guildChannel.CreatedAt}.");
            return Task.CompletedTask;
        }

        #endregion

        #region ChannelDestroyed

        public void HookChannelDestroyed(BaseSocketClient client)
            => client.ChannelDestroyed += HandleChannelDestroyed;

        public Task HandleChannelDestroyed(SocketChannel channel)
        {
            if (channel is SocketGuildChannel guildChannel)
                Console.WriteLine(
                    $"A new channel '{guildChannel.Name}'({guildChannel.Id}, {guildChannel.GetType()}) has been deleted.");
            return Task.CompletedTask;
        }

        #endregion

        #region ChannelUpdated

        public void HookChannelUpdated(BaseSocketClient client)
            => client.ChannelUpdated += HandleChannelRename;

        public Task HandleChannelRename(SocketChannel beforeChannel, SocketChannel afterChannel)
        {
            if (beforeChannel is SocketGuildChannel beforeGuildChannel &&
                afterChannel is SocketGuildChannel afterGuildChannel)
                if (beforeGuildChannel.Name != afterGuildChannel.Name)
                    Console.WriteLine(
                        $"A channel ({beforeChannel.Id}) is renamed from {beforeGuildChannel.Name} to {afterGuildChannel.Name}.");
            return Task.CompletedTask;
        }

        #endregion

        #region MessageReceived

        private readonly ulong[] _targetUserIds = {168693960628371456, 53905483156684800};

        public void HookMessageReceived(BaseSocketClient client)
            => client.MessageReceived += HandleMessageReceived;

        public Task HandleMessageReceived(SocketMessage message)
        {
            // check if the message is a user message as opposed to a system message (e.g. Clyde, pins, etc.)
            if (!(message is SocketUserMessage userMessage)) return Task.CompletedTask;
            // check if the message origin is a guild message channel
            if (!(userMessage.Channel is SocketTextChannel textChannel)) return Task.CompletedTask;
            // check if the target user was mentioned
            var targetUsers = userMessage.MentionedUsers.Where(x => _targetUserIds.Contains(x.Id));
            foreach (var targetUser in targetUsers)
                Console.WriteLine(
                    $"{targetUser} was mentioned in the message '{message.Content}' by {message.Author} in {textChannel.Name}.");
            return Task.CompletedTask;
        }

        #endregion

        #region MessageDeleted

        public void HookMessageDeleted(BaseSocketClient client)
            => client.MessageDeleted += HandleMessageDelete;

        public async Task HandleMessageDelete(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel)
        {
            // check if the message exists in cache; if not, we cannot report what was removed
            if (!cachedMessage.HasValue) return;
            // gets or downloads the channel if it's not in the cache
            IMessageChannel channel = await cachedChannel.GetOrDownloadAsync();
            var message = cachedMessage.Value;
            Console.WriteLine(
                $"A message ({message.Id}) from {message.Author} was removed from the channel {channel.Name} ({channel.Id}):"
                + Environment.NewLine
                + message.Content);
        }

        #endregion
    }
}
