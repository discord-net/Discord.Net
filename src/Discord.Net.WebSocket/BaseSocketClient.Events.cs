using Discord.Rest;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public partial class BaseSocketClient
    {
        #region Channels
        /// <summary> Fired when a channel is created. </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when a generic channel has been created. The event handler must return a
        ///         <see cref="Task"/> and accept a <see cref="SocketChannel"/> as its parameter.
        ///     </para>
        ///     <para>
        ///         The newly created channel is passed into the event handler parameter. The given channel type may
        ///         include, but not limited to, Private Channels (DM, Group), Guild Channels (Text, Voice, Category);
        ///         see the derived classes of <see cref="SocketChannel"/> for more details.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     <code language="cs" region="ChannelCreated"
        ///           source="../Discord.Net.Examples/WebSocket/BaseSocketClient.Events.Examples.cs"/>
        /// </example>
        public event Func<SocketChannel, Task> ChannelCreated
        {
            add { _channelCreatedEvent.Add(value); }
            remove { _channelCreatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketChannel, Task>> _channelCreatedEvent = new AsyncEvent<Func<SocketChannel, Task>>();
        /// <summary> Fired when a channel is destroyed. </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when a generic channel has been destroyed. The event handler must return a
        ///         <see cref="Task"/> and accept a <see cref="SocketChannel"/> as its parameter.
        ///     </para>
        ///     <para>
        ///         The destroyed channel is passed into the event handler parameter. The given channel type may
        ///         include, but not limited to, Private Channels (DM, Group), Guild Channels (Text, Voice, Category);
        ///         see the derived classes of <see cref="SocketChannel"/> for more details.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     <code language="cs" region="ChannelDestroyed"
        ///           source="../Discord.Net.Examples/WebSocket/BaseSocketClient.Events.Examples.cs"/>
        /// </example>
        public event Func<SocketChannel, Task> ChannelDestroyed
        {
            add { _channelDestroyedEvent.Add(value); }
            remove { _channelDestroyedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketChannel, Task>> _channelDestroyedEvent = new AsyncEvent<Func<SocketChannel, Task>>();
        /// <summary> Fired when a channel is updated. </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when a generic channel has been updated. The event handler must return a
        ///         <see cref="Task"/> and accept 2 <see cref="SocketChannel"/> as its parameters.
        ///     </para>
        ///     <para>
        ///         The original (prior to update) channel is passed into the first <see cref="SocketChannel"/>, while
        ///         the updated channel is passed into the second. The given channel type may include, but not limited
        ///         to, Private Channels (DM, Group), Guild Channels (Text, Voice, Category); see the derived classes of
        ///         <see cref="SocketChannel"/> for more details.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     <code language="cs" region="ChannelUpdated"
        ///           source="../Discord.Net.Examples/WebSocket/BaseSocketClient.Events.Examples.cs"/>
        /// </example>
        public event Func<SocketChannel, SocketChannel, Task> ChannelUpdated
        {
            add { _channelUpdatedEvent.Add(value); }
            remove { _channelUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketChannel, SocketChannel, Task>> _channelUpdatedEvent = new AsyncEvent<Func<SocketChannel, SocketChannel, Task>>();

        /// <summary>
        ///     Fired when status of a voice channel is updated.
        /// </summary>
        /// <remarks>
        ///     The previous state of the channel is passed into the first parameter; the updated channel is passed into the second one.
        /// </remarks>
        public event Func<Cacheable<SocketVoiceChannel, ulong>, string, string, Task> VoiceChannelStatusUpdated
        {
            add { _voiceChannelStatusUpdated.Add(value); }
            remove { _voiceChannelStatusUpdated.Remove(value); }
        }

        internal readonly AsyncEvent<Func<Cacheable<SocketVoiceChannel, ulong>, string, string, Task>> _voiceChannelStatusUpdated = new();


        #endregion

        #region Messages
        /// <summary> Fired when a message is received. </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when a message is received. The event handler must return a
        ///         <see cref="Task"/> and accept a <see cref="SocketMessage"/> as its parameter.
        ///     </para>
        ///     <para>
        ///         The message that is sent to the client is passed into the event handler parameter as
        ///         <see cref="SocketMessage"/>. This message may be a system message (i.e.
        ///         <see cref="SocketSystemMessage"/>) or a user message (i.e. <see cref="SocketUserMessage"/>. See the
        ///         derived classes of <see cref="SocketMessage"/> for more details.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     <para>The example below checks if the newly received message contains the target user.</para>
        ///     <code language="cs" region="MessageReceived"
        ///           source="../Discord.Net.Examples/WebSocket/BaseSocketClient.Events.Examples.cs"/>
        /// </example>
        public event Func<SocketMessage, Task> MessageReceived
        {
            add { _messageReceivedEvent.Add(value); }
            remove { _messageReceivedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketMessage, Task>> _messageReceivedEvent = new AsyncEvent<Func<SocketMessage, Task>>();
        /// <summary> Fired when a message is deleted. </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when a message is deleted. The event handler must return a
        ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/> and
        ///         <see cref="ISocketMessageChannel"/> as its parameters.
        ///     </para>
        ///     <para>
        ///         <note type="important">
        ///             It is not possible to retrieve the message via
        ///             <see cref="Cacheable{TEntity,TId}.DownloadAsync"/>; the message cannot be retrieved by Discord
        ///             after the message has been deleted.
        ///         </note>
        ///         If caching is enabled via <see cref="DiscordSocketConfig"/>, the
        ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the deleted message; otherwise, in event
        ///         that the message cannot be retrieved, the snowflake ID of the message is preserved in the
        ///         <see cref="ulong"/>.
        ///     </para>
        ///     <para>
        ///         The source channel of the removed message will be passed into the
        ///         <see cref="ISocketMessageChannel"/> parameter.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     <code language="cs" region="MessageDeleted"
        ///           source="../Discord.Net.Examples/WebSocket/BaseSocketClient.Events.Examples.cs" />
        /// </example>

        public event Func<Cacheable<IMessage, ulong>, Cacheable<IMessageChannel, ulong>, Task> MessageDeleted
        {
            add { _messageDeletedEvent.Add(value); }
            remove { _messageDeletedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<IMessage, ulong>, Cacheable<IMessageChannel, ulong>, Task>> _messageDeletedEvent = new AsyncEvent<Func<Cacheable<IMessage, ulong>, Cacheable<IMessageChannel, ulong>, Task>>();
        /// <summary> Fired when multiple messages are bulk deleted. </summary>
        /// <remarks>
        ///     <note>
        ///         The <see cref="MessageDeleted"/> event will not be fired for individual messages contained in this event.
        ///     </note>
        ///     <para>
        ///         This event is fired when multiple messages are bulk deleted. The event handler must return a
        ///         <see cref="Task"/> and accept an <see cref="IReadOnlyCollection{Cacheable}"/> and
        ///         <see cref="ISocketMessageChannel"/> as its parameters.
        ///     </para>
        ///     <para>
        ///         <note type="important">
        ///             It is not possible to retrieve the message via
        ///             <see cref="Cacheable{TEntity,TId}.DownloadAsync"/>; the message cannot be retrieved by Discord
        ///             after the message has been deleted.
        ///         </note>
        ///         If caching is enabled via <see cref="DiscordSocketConfig"/>, the
        ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the deleted message; otherwise, in event
        ///         that the message cannot be retrieved, the snowflake ID of the message is preserved in the
        ///         <see cref="ulong"/>.
        ///     </para>
        ///     <para>
        ///         The source channel of the removed message will be passed into the
        ///         <see cref="ISocketMessageChannel"/> parameter.
        ///     </para>
        /// </remarks>
        public event Func<IReadOnlyCollection<Cacheable<IMessage, ulong>>, Cacheable<IMessageChannel, ulong>, Task> MessagesBulkDeleted
        {
            add { _messagesBulkDeletedEvent.Add(value); }
            remove { _messagesBulkDeletedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<IReadOnlyCollection<Cacheable<IMessage, ulong>>, Cacheable<IMessageChannel, ulong>, Task>> _messagesBulkDeletedEvent = new AsyncEvent<Func<IReadOnlyCollection<Cacheable<IMessage, ulong>>, Cacheable<IMessageChannel, ulong>, Task>>();
        /// <summary> Fired when a message is updated. </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when a message is updated. The event handler must return a
        ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, <see cref="SocketMessage"/>,
        ///         and <see cref="ISocketMessageChannel"/> as its parameters.
        ///     </para>
        ///     <para>
        ///         If caching is enabled via <see cref="DiscordSocketConfig"/>, the
        ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
        ///         that the message cannot be retrieved, the snowflake ID of the message is preserved in the
        ///         <see cref="ulong"/>.
        ///     </para>
        ///     <para>
        ///         The updated message will be passed into the <see cref="SocketMessage"/> parameter.
        ///     </para>
        ///     <para>
        ///         The source channel of the updated message will be passed into the
        ///         <see cref="ISocketMessageChannel"/> parameter.
        ///     </para>
        /// </remarks>
        public event Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated
        {
            add { _messageUpdatedEvent.Add(value); }
            remove { _messageUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task>> _messageUpdatedEvent = new AsyncEvent<Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task>>();
        /// <summary> Fired when a reaction is added to a message. </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when a reaction is added to a user message. The event handler must return a
        ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, an
        ///         <see cref="ISocketMessageChannel"/>, and a <see cref="SocketReaction"/> as its parameter.
        ///     </para>
        ///     <para>
        ///         If caching is enabled via <see cref="DiscordSocketConfig"/>, the
        ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
        ///         that the message cannot be retrieved, the snowflake ID of the message is preserved in the
        ///         <see cref="ulong"/>.
        ///     </para>
        ///     <para>
        ///         The source channel of the reaction addition will be passed into the
        ///         <see cref="ISocketMessageChannel"/> parameter.
        ///     </para>
        ///     <para>
        ///         The reaction that was added will be passed into the <see cref="SocketReaction"/> parameter.
        ///     </para>
        ///     <note>
        ///         When fetching the reaction from this event, a user may not be provided under
        ///         <see cref="SocketReaction.User"/>. Please see the documentation of the property for more
        ///         information.
        ///     </note>
        /// </remarks>
        /// <example>
        ///     <code language="cs" region="ReactionAdded"
        ///           source="../Discord.Net.Examples/WebSocket/BaseSocketClient.Events.Examples.cs"/>
        /// </example>
        public event Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task> ReactionAdded
        {
            add { _reactionAddedEvent.Add(value); }
            remove { _reactionAddedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task>> _reactionAddedEvent = new AsyncEvent<Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task>>();
        /// <summary> Fired when a reaction is removed from a message. </summary>
        public event Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task> ReactionRemoved
        {
            add { _reactionRemovedEvent.Add(value); }
            remove { _reactionRemovedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task>> _reactionRemovedEvent = new AsyncEvent<Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task>>();
        /// <summary> Fired when all reactions to a message are cleared. </summary>
        public event Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, Task> ReactionsCleared
        {
            add { _reactionsClearedEvent.Add(value); }
            remove { _reactionsClearedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, Task>> _reactionsClearedEvent = new AsyncEvent<Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, Task>>();
        /// <summary>
        ///     Fired when all reactions to a message with a specific emote are removed.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when all reactions to a message with a specific emote are removed.
        ///         The event handler must return a <see cref="Task"/> and accept a <see cref="ISocketMessageChannel"/> and
        ///         a <see cref="IEmote"/> as its parameters.
        ///     </para>
        ///     <para>
        ///         The channel where this message was sent will be passed into the <see cref="ISocketMessageChannel"/> parameter.
        ///     </para>
        ///     <para>
        ///         The emoji that all reactions had and were removed will be passed into the <see cref="IEmote"/> parameter.
        ///     </para>
        /// </remarks>
        public event Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, IEmote, Task> ReactionsRemovedForEmote
        {
            add { _reactionsRemovedForEmoteEvent.Add(value); }
            remove { _reactionsRemovedForEmoteEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, IEmote, Task>> _reactionsRemovedForEmoteEvent = new AsyncEvent<Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, IEmote, Task>>();
        #endregion

        #region Polls

        /// <summary>
        ///     Fired when a vote is added to a poll.
        /// </summary>
        public event Func<Cacheable<IUser, ulong>, Cacheable<ISocketMessageChannel, IRestMessageChannel, IMessageChannel, ulong>, Cacheable<IUserMessage, ulong>, Cacheable<SocketGuild, RestGuild, IGuild, ulong>?, ulong, Task> PollVoteAdded
        {
            add { _pollVoteAdded.Add(value); }
            remove { _pollVoteAdded.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<IUser, ulong>, Cacheable<ISocketMessageChannel, IRestMessageChannel, IMessageChannel, ulong>, Cacheable<IUserMessage, ulong>, Cacheable<SocketGuild, RestGuild, IGuild, ulong>?, ulong, Task>> _pollVoteAdded = new ();

        /// <summary>
        ///     Fired when a vote is removed from a poll.
        /// </summary>
        public event Func<Cacheable< IUser, ulong>, Cacheable<ISocketMessageChannel, IRestMessageChannel, IMessageChannel, ulong>, Cacheable<IUserMessage, ulong>, Cacheable<SocketGuild, RestGuild, IGuild, ulong>?, ulong, Task> PollVoteRemoved
        {
            add { _pollVoteRemoved.Add(value); }
            remove { _pollVoteRemoved.Remove(value); }
        }

        internal readonly AsyncEvent<Func<Cacheable<IUser, ulong>, Cacheable<ISocketMessageChannel, IRestMessageChannel, IMessageChannel, ulong>, Cacheable<IUserMessage, ulong>, Cacheable<SocketGuild, RestGuild, IGuild, ulong>?, ulong, Task>> _pollVoteRemoved = new ();

        #endregion

        #region Roles
        /// <summary> Fired when a role is created. </summary>
        public event Func<SocketRole, Task> RoleCreated
        {
            add { _roleCreatedEvent.Add(value); }
            remove { _roleCreatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketRole, Task>> _roleCreatedEvent = new AsyncEvent<Func<SocketRole, Task>>();
        /// <summary> Fired when a role is deleted. </summary>
        public event Func<SocketRole, Task> RoleDeleted
        {
            add { _roleDeletedEvent.Add(value); }
            remove { _roleDeletedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketRole, Task>> _roleDeletedEvent = new AsyncEvent<Func<SocketRole, Task>>();
        /// <summary> Fired when a role is updated. </summary>
        public event Func<SocketRole, SocketRole, Task> RoleUpdated
        {
            add { _roleUpdatedEvent.Add(value); }
            remove { _roleUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketRole, SocketRole, Task>> _roleUpdatedEvent = new AsyncEvent<Func<SocketRole, SocketRole, Task>>();
        #endregion

        #region Guilds
        /// <summary> Fired when the connected account joins a guild. </summary>
        public event Func<SocketGuild, Task> JoinedGuild
        {
            add { _joinedGuildEvent.Add(value); }
            remove { _joinedGuildEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, Task>> _joinedGuildEvent = new AsyncEvent<Func<SocketGuild, Task>>();
        /// <summary> Fired when the connected account leaves a guild. </summary>
        public event Func<SocketGuild, Task> LeftGuild
        {
            add { _leftGuildEvent.Add(value); }
            remove { _leftGuildEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, Task>> _leftGuildEvent = new AsyncEvent<Func<SocketGuild, Task>>();
        /// <summary> Fired when a guild becomes available. </summary>
        public event Func<SocketGuild, Task> GuildAvailable
        {
            add { _guildAvailableEvent.Add(value); }
            remove { _guildAvailableEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildAvailableEvent = new AsyncEvent<Func<SocketGuild, Task>>();
        /// <summary> Fired when a guild becomes unavailable. </summary>
        public event Func<SocketGuild, Task> GuildUnavailable
        {
            add { _guildUnavailableEvent.Add(value); }
            remove { _guildUnavailableEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildUnavailableEvent = new AsyncEvent<Func<SocketGuild, Task>>();
        /// <summary> Fired when offline guild members are downloaded. </summary>
        public event Func<SocketGuild, Task> GuildMembersDownloaded
        {
            add { _guildMembersDownloadedEvent.Add(value); }
            remove { _guildMembersDownloadedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildMembersDownloadedEvent = new AsyncEvent<Func<SocketGuild, Task>>();
        /// <summary> Fired when a guild is updated. </summary>
        public event Func<SocketGuild, SocketGuild, Task> GuildUpdated
        {
            add { _guildUpdatedEvent.Add(value); }
            remove { _guildUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, SocketGuild, Task>> _guildUpdatedEvent = new AsyncEvent<Func<SocketGuild, SocketGuild, Task>>();
        /// <summary>Fired when a user leaves without agreeing to the member screening </summary>
        public event Func<Cacheable<SocketGuildUser, ulong>, SocketGuild, Task> GuildJoinRequestDeleted
        {
            add { _guildJoinRequestDeletedEvent.Add(value); }
            remove { _guildJoinRequestDeletedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, SocketGuild, Task>> _guildJoinRequestDeletedEvent = new AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, SocketGuild, Task>>();
        #endregion

        #region Guild Events

        /// <summary>
        ///     Fired when a guild event is created.
        /// </summary>
        public event Func<SocketGuildEvent, Task> GuildScheduledEventCreated
        {
            add { _guildScheduledEventCreated.Add(value); }
            remove { _guildScheduledEventCreated.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuildEvent, Task>> _guildScheduledEventCreated = new AsyncEvent<Func<SocketGuildEvent, Task>>();

        /// <summary>
        ///     Fired when a guild event is updated.
        /// </summary>
        public event Func<Cacheable<SocketGuildEvent, ulong>, SocketGuildEvent, Task> GuildScheduledEventUpdated
        {
            add { _guildScheduledEventUpdated.Add(value); }
            remove { _guildScheduledEventUpdated.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<SocketGuildEvent, ulong>, SocketGuildEvent, Task>> _guildScheduledEventUpdated = new AsyncEvent<Func<Cacheable<SocketGuildEvent, ulong>, SocketGuildEvent, Task>>();


        /// <summary>
        ///     Fired when a guild event is cancelled.
        /// </summary>
        public event Func<SocketGuildEvent, Task> GuildScheduledEventCancelled
        {
            add { _guildScheduledEventCancelled.Add(value); }
            remove { _guildScheduledEventCancelled.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuildEvent, Task>> _guildScheduledEventCancelled = new AsyncEvent<Func<SocketGuildEvent, Task>>();

        /// <summary>
        ///     Fired when a guild event is completed.
        /// </summary>
        public event Func<SocketGuildEvent, Task> GuildScheduledEventCompleted
        {
            add { _guildScheduledEventCompleted.Add(value); }
            remove { _guildScheduledEventCompleted.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuildEvent, Task>> _guildScheduledEventCompleted = new AsyncEvent<Func<SocketGuildEvent, Task>>();

        /// <summary>
        ///     Fired when a guild event is started.
        /// </summary>
        public event Func<SocketGuildEvent, Task> GuildScheduledEventStarted
        {
            add { _guildScheduledEventStarted.Add(value); }
            remove { _guildScheduledEventStarted.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuildEvent, Task>> _guildScheduledEventStarted = new AsyncEvent<Func<SocketGuildEvent, Task>>();

        public event Func<Cacheable<SocketUser, RestUser, IUser, ulong>, SocketGuildEvent, Task> GuildScheduledEventUserAdd
        {
            add { _guildScheduledEventUserAdd.Add(value); }
            remove { _guildScheduledEventUserAdd.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<SocketUser, RestUser, IUser, ulong>, SocketGuildEvent, Task>> _guildScheduledEventUserAdd = new AsyncEvent<Func<Cacheable<SocketUser, RestUser, IUser, ulong>, SocketGuildEvent, Task>>();

        public event Func<Cacheable<SocketUser, RestUser, IUser, ulong>, SocketGuildEvent, Task> GuildScheduledEventUserRemove
        {
            add { _guildScheduledEventUserRemove.Add(value); }
            remove { _guildScheduledEventUserRemove.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<SocketUser, RestUser, IUser, ulong>, SocketGuildEvent, Task>> _guildScheduledEventUserRemove = new AsyncEvent<Func<Cacheable<SocketUser, RestUser, IUser, ulong>, SocketGuildEvent, Task>>();


        #endregion

        #region Integrations
        /// <summary> Fired when an integration is created. </summary>
        public event Func<IIntegration, Task> IntegrationCreated
        {
            add { _integrationCreated.Add(value); }
            remove { _integrationCreated.Remove(value); }
        }
        internal readonly AsyncEvent<Func<IIntegration, Task>> _integrationCreated = new AsyncEvent<Func<IIntegration, Task>>();

        /// <summary> Fired when an integration is updated. </summary>
        public event Func<IIntegration, Task> IntegrationUpdated
        {
            add { _integrationUpdated.Add(value); }
            remove { _integrationUpdated.Remove(value); }
        }
        internal readonly AsyncEvent<Func<IIntegration, Task>> _integrationUpdated = new AsyncEvent<Func<IIntegration, Task>>();

        /// <summary> Fired when an integration is deleted. </summary>
        public event Func<IGuild, ulong, Optional<ulong>, Task> IntegrationDeleted
        {
            add { _integrationDeleted.Add(value); }
            remove { _integrationDeleted.Remove(value); }
        }
        internal readonly AsyncEvent<Func<IGuild, ulong, Optional<ulong>, Task>> _integrationDeleted = new AsyncEvent<Func<IGuild, ulong, Optional<ulong>, Task>>();
        #endregion

        #region Users
        /// <summary> Fired when a user joins a guild. </summary>
        public event Func<SocketGuildUser, Task> UserJoined
        {
            add { _userJoinedEvent.Add(value); }
            remove { _userJoinedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuildUser, Task>> _userJoinedEvent = new AsyncEvent<Func<SocketGuildUser, Task>>();
        /// <summary> Fired when a user leaves a guild. </summary>
        public event Func<SocketGuild, SocketUser, Task> UserLeft
        {
            add { _userLeftEvent.Add(value); }
            remove { _userLeftEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, SocketUser, Task>> _userLeftEvent = new AsyncEvent<Func<SocketGuild, SocketUser, Task>>();
        /// <summary> Fired when a user is banned from a guild. </summary>
        public event Func<SocketUser, SocketGuild, Task> UserBanned
        {
            add { _userBannedEvent.Add(value); }
            remove { _userBannedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketUser, SocketGuild, Task>> _userBannedEvent = new AsyncEvent<Func<SocketUser, SocketGuild, Task>>();
        /// <summary> Fired when a user is unbanned from a guild. </summary>
        public event Func<SocketUser, SocketGuild, Task> UserUnbanned
        {
            add { _userUnbannedEvent.Add(value); }
            remove { _userUnbannedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketUser, SocketGuild, Task>> _userUnbannedEvent = new AsyncEvent<Func<SocketUser, SocketGuild, Task>>();
        /// <summary> Fired when a user is updated. </summary>
        public event Func<SocketUser, SocketUser, Task> UserUpdated
        {
            add { _userUpdatedEvent.Add(value); }
            remove { _userUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketUser, SocketUser, Task>> _userUpdatedEvent = new AsyncEvent<Func<SocketUser, SocketUser, Task>>();
        /// <summary> Fired when a guild member is updated. </summary>
        public event Func<Cacheable<SocketGuildUser, ulong>, SocketGuildUser, Task> GuildMemberUpdated
        {
            add { _guildMemberUpdatedEvent.Add(value); }
            remove { _guildMemberUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, SocketGuildUser, Task>> _guildMemberUpdatedEvent = new AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, SocketGuildUser, Task>>();
        /// <summary> Fired when a user joins, leaves, or moves voice channels. </summary>
        public event Func<SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated
        {
            add { _userVoiceStateUpdatedEvent.Add(value); }
            remove { _userVoiceStateUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketUser, SocketVoiceState, SocketVoiceState, Task>> _userVoiceStateUpdatedEvent = new AsyncEvent<Func<SocketUser, SocketVoiceState, SocketVoiceState, Task>>();
        /// <summary> Fired when the bot connects to a Discord voice server. </summary>
        public event Func<SocketVoiceServer, Task> VoiceServerUpdated
        {
            add { _voiceServerUpdatedEvent.Add(value); }
            remove { _voiceServerUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketVoiceServer, Task>> _voiceServerUpdatedEvent = new AsyncEvent<Func<SocketVoiceServer, Task>>();
        /// <summary> Fired when the connected account is updated. </summary>
        public event Func<SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated
        {
            add { _selfUpdatedEvent.Add(value); }
            remove { _selfUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketSelfUser, SocketSelfUser, Task>> _selfUpdatedEvent = new AsyncEvent<Func<SocketSelfUser, SocketSelfUser, Task>>();
        /// <summary> Fired when a user starts typing. </summary>
        public event Func<Cacheable<IUser, ulong>, Cacheable<IMessageChannel, ulong>, Task> UserIsTyping
        {
            add { _userIsTypingEvent.Add(value); }
            remove { _userIsTypingEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<IUser, ulong>, Cacheable<IMessageChannel, ulong>, Task>> _userIsTypingEvent = new AsyncEvent<Func<Cacheable<IUser, ulong>, Cacheable<IMessageChannel, ulong>, Task>>();
        /// <summary> Fired when a user joins a group channel. </summary>
        public event Func<SocketGroupUser, Task> RecipientAdded
        {
            add { _recipientAddedEvent.Add(value); }
            remove { _recipientAddedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGroupUser, Task>> _recipientAddedEvent = new AsyncEvent<Func<SocketGroupUser, Task>>();
        /// <summary> Fired when a user is removed from a group channel. </summary>
        public event Func<SocketGroupUser, Task> RecipientRemoved
        {
            add { _recipientRemovedEvent.Add(value); }
            remove { _recipientRemovedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGroupUser, Task>> _recipientRemovedEvent = new AsyncEvent<Func<SocketGroupUser, Task>>();
        #endregion

        #region Presence

        /// <summary> Fired when a users presence is updated. </summary>
        public event Func<SocketUser, SocketPresence, SocketPresence, Task> PresenceUpdated
        {
            add { _presenceUpdated.Add(value); }
            remove { _presenceUpdated.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketUser, SocketPresence, SocketPresence, Task>> _presenceUpdated = new AsyncEvent<Func<SocketUser, SocketPresence, SocketPresence, Task>>();

        #endregion

        #region Invites
        /// <summary>
        ///     Fired when an invite is created.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when an invite is created. The event handler must return a
        ///         <see cref="Task"/> and accept a <see cref="SocketInvite"/> as its parameter.
        ///     </para>
        ///     <para>
        ///         The invite created will be passed into the <see cref="SocketInvite"/> parameter.
        ///     </para>
        /// </remarks>
        public event Func<SocketInvite, Task> InviteCreated
        {
            add { _inviteCreatedEvent.Add(value); }
            remove { _inviteCreatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketInvite, Task>> _inviteCreatedEvent = new AsyncEvent<Func<SocketInvite, Task>>();
        /// <summary>
        ///     Fired when an invite is deleted.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when an invite is deleted. The event handler must return
        ///         a <see cref="Task"/> and accept a <see cref="SocketGuildChannel"/> and
        ///         <see cref="string"/> as its parameter.
        ///     </para>
        ///     <para>
        ///         The channel where this invite was created will be passed into the <see cref="SocketGuildChannel"/> parameter.
        ///     </para>
        ///     <para>
        ///         The code of the deleted invite will be passed into the <see cref="string"/> parameter.
        ///     </para>
        /// </remarks>
        public event Func<SocketGuildChannel, string, Task> InviteDeleted
        {
            add { _inviteDeletedEvent.Add(value); }
            remove { _inviteDeletedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuildChannel, string, Task>> _inviteDeletedEvent = new AsyncEvent<Func<SocketGuildChannel, string, Task>>();
        #endregion

        #region Interactions
        /// <summary>
        ///     Fired when an Interaction is created. This event covers all types of interactions including but not limited to: buttons, select menus, slash commands, autocompletes.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when an interaction is created. The event handler must return a
        ///         <see cref="Task"/> and accept a <see cref="SocketInteraction"/> as its parameter.
        ///     </para>
        ///     <para>
        ///         The interaction created will be passed into the <see cref="SocketInteraction"/> parameter.
        ///     </para>
        /// </remarks>
        public event Func<SocketInteraction, Task> InteractionCreated
        {
            add { _interactionCreatedEvent.Add(value); }
            remove { _interactionCreatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketInteraction, Task>> _interactionCreatedEvent = new AsyncEvent<Func<SocketInteraction, Task>>();

        /// <summary>
        ///     Fired when a button is clicked and its interaction is received.
        /// </summary>
        public event Func<SocketMessageComponent, Task> ButtonExecuted
        {
            add => _buttonExecuted.Add(value);
            remove => _buttonExecuted.Remove(value);
        }
        internal readonly AsyncEvent<Func<SocketMessageComponent, Task>> _buttonExecuted = new AsyncEvent<Func<SocketMessageComponent, Task>>();

        /// <summary>
        ///     Fired when a select menu is used and its interaction is received.
        /// </summary>
        public event Func<SocketMessageComponent, Task> SelectMenuExecuted
        {
            add => _selectMenuExecuted.Add(value);
            remove => _selectMenuExecuted.Remove(value);
        }
        internal readonly AsyncEvent<Func<SocketMessageComponent, Task>> _selectMenuExecuted = new AsyncEvent<Func<SocketMessageComponent, Task>>();
        /// <summary>
        ///     Fired when a slash command is used and its interaction is received.
        /// </summary>
        public event Func<SocketSlashCommand, Task> SlashCommandExecuted
        {
            add => _slashCommandExecuted.Add(value);
            remove => _slashCommandExecuted.Remove(value);
        }
        internal readonly AsyncEvent<Func<SocketSlashCommand, Task>> _slashCommandExecuted = new AsyncEvent<Func<SocketSlashCommand, Task>>();

        /// <summary>
        ///     Fired when a user command is used and its interaction is received.
        /// </summary>
        public event Func<SocketUserCommand, Task> UserCommandExecuted
        {
            add => _userCommandExecuted.Add(value);
            remove => _userCommandExecuted.Remove(value);
        }
        internal readonly AsyncEvent<Func<SocketUserCommand, Task>> _userCommandExecuted = new AsyncEvent<Func<SocketUserCommand, Task>>();

        /// <summary>
        ///     Fired when a message command is used and its interaction is received.
        /// </summary>
        public event Func<SocketMessageCommand, Task> MessageCommandExecuted
        {
            add => _messageCommandExecuted.Add(value);
            remove => _messageCommandExecuted.Remove(value);
        }
        internal readonly AsyncEvent<Func<SocketMessageCommand, Task>> _messageCommandExecuted = new AsyncEvent<Func<SocketMessageCommand, Task>>();
        /// <summary>
        ///     Fired when an autocomplete is used and its interaction is received.
        /// </summary>
        public event Func<SocketAutocompleteInteraction, Task> AutocompleteExecuted
        {
            add => _autocompleteExecuted.Add(value);
            remove => _autocompleteExecuted.Remove(value);
        }
        internal readonly AsyncEvent<Func<SocketAutocompleteInteraction, Task>> _autocompleteExecuted = new AsyncEvent<Func<SocketAutocompleteInteraction, Task>>();
        /// <summary>
        ///     Fired when a modal is submitted.
        /// </summary>
        public event Func<SocketModal, Task> ModalSubmitted
        {
            add => _modalSubmitted.Add(value);
            remove => _modalSubmitted.Remove(value);
        }
        internal readonly AsyncEvent<Func<SocketModal, Task>> _modalSubmitted = new AsyncEvent<Func<SocketModal, Task>>();

        /// <summary>
        ///     Fired when a guild application command is created.
        ///</summary>
        ///<remarks>
        ///     <para>
        ///         This event is fired when an application command is created. The event handler must return a
        ///         <see cref="Task"/> and accept a <see cref="SocketApplicationCommand"/> as its parameter.
        ///     </para>
        ///     <para>
        ///         The command that was deleted will be passed into the <see cref="SocketApplicationCommand"/> parameter.
        ///     </para>
        ///     <note>
        ///         <b>This event is an undocumented discord event and may break at any time, its not recommended to rely on this event</b>
        ///     </note>
        /// </remarks>
        public event Func<SocketApplicationCommand, Task> ApplicationCommandCreated
        {
            add { _applicationCommandCreated.Add(value); }
            remove { _applicationCommandCreated.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketApplicationCommand, Task>> _applicationCommandCreated = new AsyncEvent<Func<SocketApplicationCommand, Task>>();

        /// <summary>
        ///      Fired when a guild application command is updated.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when an application command is updated. The event handler must return a
        ///         <see cref="Task"/> and accept a <see cref="SocketApplicationCommand"/> as its parameter.
        ///     </para>
        ///     <para>
        ///         The command that was deleted will be passed into the <see cref="SocketApplicationCommand"/> parameter.
        ///     </para>
        ///     <note>
        ///         <b>This event is an undocumented discord event and may break at any time, its not recommended to rely on this event</b>
        ///     </note>
        /// </remarks>
        public event Func<SocketApplicationCommand, Task> ApplicationCommandUpdated
        {
            add { _applicationCommandUpdated.Add(value); }
            remove { _applicationCommandUpdated.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketApplicationCommand, Task>> _applicationCommandUpdated = new AsyncEvent<Func<SocketApplicationCommand, Task>>();

        /// <summary>
        ///      Fired when a guild application command is deleted.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when an application command is deleted. The event handler must return a
        ///         <see cref="Task"/> and accept a <see cref="SocketApplicationCommand"/> as its parameter.
        ///     </para>
        ///     <para>
        ///         The command that was deleted will be passed into the <see cref="SocketApplicationCommand"/> parameter.
        ///     </para>
        ///     <note>
        ///         <b>This event is an undocumented discord event and may break at any time, its not recommended to rely on this event</b>
        ///     </note>
        /// </remarks>
        public event Func<SocketApplicationCommand, Task> ApplicationCommandDeleted
        {
            add { _applicationCommandDeleted.Add(value); }
            remove { _applicationCommandDeleted.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketApplicationCommand, Task>> _applicationCommandDeleted = new AsyncEvent<Func<SocketApplicationCommand, Task>>();

        /// <summary>
        ///     Fired when a thread is created within a guild, or when the current user is added to a thread.
        /// </summary>
        public event Func<SocketThreadChannel, Task> ThreadCreated
        {
            add { _threadCreated.Add(value); }
            remove { _threadCreated.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketThreadChannel, Task>> _threadCreated = new AsyncEvent<Func<SocketThreadChannel, Task>>();

        /// <summary>
        ///     Fired when a thread is updated within a guild.
        /// </summary>
        public event Func<Cacheable<SocketThreadChannel, ulong>, SocketThreadChannel, Task> ThreadUpdated
        {
            add { _threadUpdated.Add(value); }
            remove { _threadUpdated.Remove(value); }
        }

        internal readonly AsyncEvent<Func<Cacheable<SocketThreadChannel, ulong>, SocketThreadChannel, Task>> _threadUpdated = new();

        /// <summary>
        ///     Fired when a thread is deleted.
        /// </summary>
        public event Func<Cacheable<SocketThreadChannel, ulong>, Task> ThreadDeleted
        {
            add { _threadDeleted.Add(value); }
            remove { _threadDeleted.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<SocketThreadChannel, ulong>, Task>> _threadDeleted = new AsyncEvent<Func<Cacheable<SocketThreadChannel, ulong>, Task>>();

        /// <summary>
        ///     Fired when a user joins a thread
        /// </summary>
        public event Func<SocketThreadUser, Task> ThreadMemberJoined
        {
            add { _threadMemberJoined.Add(value); }
            remove { _threadMemberJoined.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketThreadUser, Task>> _threadMemberJoined = new AsyncEvent<Func<SocketThreadUser, Task>>();

        /// <summary>
        ///     Fired when a user leaves a thread
        /// </summary>
        public event Func<SocketThreadUser, Task> ThreadMemberLeft
        {
            add { _threadMemberLeft.Add(value); }
            remove { _threadMemberLeft.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketThreadUser, Task>> _threadMemberLeft = new AsyncEvent<Func<SocketThreadUser, Task>>();

        /// <summary>
        ///     Fired when a stage is started.
        /// </summary>
        public event Func<SocketStageChannel, Task> StageStarted
        {
            add { _stageStarted.Add(value); }
            remove { _stageStarted.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketStageChannel, Task>> _stageStarted = new AsyncEvent<Func<SocketStageChannel, Task>>();

        /// <summary>
        ///     Fired when a stage ends.
        /// </summary>
        public event Func<SocketStageChannel, Task> StageEnded
        {
            add { _stageEnded.Add(value); }
            remove { _stageEnded.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketStageChannel, Task>> _stageEnded = new AsyncEvent<Func<SocketStageChannel, Task>>();

        /// <summary>
        ///     Fired when a stage is updated.
        /// </summary>
        public event Func<SocketStageChannel, SocketStageChannel, Task> StageUpdated
        {
            add { _stageUpdated.Add(value); }
            remove { _stageUpdated.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketStageChannel, SocketStageChannel, Task>> _stageUpdated = new AsyncEvent<Func<SocketStageChannel, SocketStageChannel, Task>>();

        /// <summary>
        ///     Fired when a user requests to speak within a stage channel.
        /// </summary>
        public event Func<SocketStageChannel, SocketGuildUser, Task> RequestToSpeak
        {
            add { _requestToSpeak.Add(value); }
            remove { _requestToSpeak.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketStageChannel, SocketGuildUser, Task>> _requestToSpeak = new AsyncEvent<Func<SocketStageChannel, SocketGuildUser, Task>>();

        /// <summary>
        ///     Fired when a speaker is added in a stage channel.
        /// </summary>
        public event Func<SocketStageChannel, SocketGuildUser, Task> SpeakerAdded
        {
            add { _speakerAdded.Add(value); }
            remove { _speakerAdded.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketStageChannel, SocketGuildUser, Task>> _speakerAdded = new AsyncEvent<Func<SocketStageChannel, SocketGuildUser, Task>>();

        /// <summary>
        ///     Fired when a speaker is removed from a stage channel.
        /// </summary>
        public event Func<SocketStageChannel, SocketGuildUser, Task> SpeakerRemoved
        {
            add { _speakerRemoved.Add(value); }
            remove { _speakerRemoved.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketStageChannel, SocketGuildUser, Task>> _speakerRemoved = new AsyncEvent<Func<SocketStageChannel, SocketGuildUser, Task>>();

        /// <summary>
        ///     Fired when a sticker in a guild is created.
        /// </summary>
        public event Func<SocketCustomSticker, Task> GuildStickerCreated
        {
            add { _guildStickerCreated.Add(value); }
            remove { _guildStickerCreated.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketCustomSticker, Task>> _guildStickerCreated = new AsyncEvent<Func<SocketCustomSticker, Task>>();

        /// <summary>
        ///     Fired when a sticker in a guild is updated.
        /// </summary>
        public event Func<SocketCustomSticker, SocketCustomSticker, Task> GuildStickerUpdated
        {
            add { _guildStickerUpdated.Add(value); }
            remove { _guildStickerUpdated.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketCustomSticker, SocketCustomSticker, Task>> _guildStickerUpdated = new AsyncEvent<Func<SocketCustomSticker, SocketCustomSticker, Task>>();

        /// <summary>
        ///     Fired when a sticker in a guild is deleted.
        /// </summary>
        public event Func<SocketCustomSticker, Task> GuildStickerDeleted
        {
            add { _guildStickerDeleted.Add(value); }
            remove { _guildStickerDeleted.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketCustomSticker, Task>> _guildStickerDeleted = new AsyncEvent<Func<SocketCustomSticker, Task>>();
        #endregion

        #region Webhooks

        /// <summary>
        ///     Fired when a webhook is modified, moved, or deleted. If the webhook was
        ///     moved the channel represents the destination channel, not the source.
        /// </summary>
        public event Func<SocketGuild, SocketChannel, Task> WebhooksUpdated
        {
            add { _webhooksUpdated.Add(value); }
            remove { _webhooksUpdated.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, SocketChannel, Task>> _webhooksUpdated = new AsyncEvent<Func<SocketGuild, SocketChannel, Task>>();

        #endregion

        #region Audit Logs

        /// <summary>
        ///     Fired when a guild audit log entry is created.
        /// </summary>
        public event Func<SocketAuditLogEntry, SocketGuild, Task> AuditLogCreated
        {
            add { _auditLogCreated.Add(value); }
            remove { _auditLogCreated.Remove(value); }
        }

        internal readonly AsyncEvent<Func<SocketAuditLogEntry, SocketGuild, Task>> _auditLogCreated = new();

        #endregion

        #region AutoModeration

        /// <summary>
        ///     Fired when an auto moderation rule is created.
        /// </summary>
        public event Func<SocketAutoModRule, Task> AutoModRuleCreated
        {
            add => _autoModRuleCreated.Add(value);
            remove => _autoModRuleCreated.Remove(value);
        }
        internal readonly AsyncEvent<Func<SocketAutoModRule, Task>> _autoModRuleCreated = new();

        /// <summary>
        ///     Fired when an auto moderation rule is modified.
        /// </summary>
        public event Func<Cacheable<SocketAutoModRule, ulong>, SocketAutoModRule, Task> AutoModRuleUpdated
        {
            add => _autoModRuleUpdated.Add(value);
            remove => _autoModRuleUpdated.Remove(value);
        }
        internal readonly AsyncEvent<Func<Cacheable<SocketAutoModRule, ulong>, SocketAutoModRule, Task>> _autoModRuleUpdated = new();

        /// <summary>
        ///     Fired when an auto moderation rule is deleted.
        /// </summary>
        public event Func<SocketAutoModRule, Task> AutoModRuleDeleted
        {
            add => _autoModRuleDeleted.Add(value);
            remove => _autoModRuleDeleted.Remove(value);
        }
        internal readonly AsyncEvent<Func<SocketAutoModRule, Task>> _autoModRuleDeleted = new();

        /// <summary>
        ///     Fired when an auto moderation rule is triggered by a user.
        /// </summary>
        public event Func<SocketGuild, AutoModRuleAction, AutoModActionExecutedData, Task> AutoModActionExecuted
        {
            add => _autoModActionExecuted.Add(value);
            remove => _autoModActionExecuted.Remove(value);
        }
        internal readonly AsyncEvent<Func<SocketGuild, AutoModRuleAction, AutoModActionExecutedData, Task>> _autoModActionExecuted = new();

        #endregion

        #region App Subscriptions

        /// <summary>
        ///     Fired when a user subscribes to a SKU.
        /// </summary>
        public event Func<SocketEntitlement, Task> EntitlementCreated
        {
            add { _entitlementCreated.Add(value); }
            remove { _entitlementCreated.Remove(value); }
        }

        internal readonly AsyncEvent<Func<SocketEntitlement, Task>> _entitlementCreated = new();


        /// <summary>
        ///     Fired when a subscription to a SKU is updated.
        /// </summary>
        public event Func<Cacheable<SocketEntitlement, ulong>, SocketEntitlement, Task> EntitlementUpdated
        {
            add { _entitlementUpdated.Add(value); }
            remove { _entitlementUpdated.Remove(value); }
        }

        internal readonly AsyncEvent<Func<Cacheable<SocketEntitlement, ulong>, SocketEntitlement, Task>> _entitlementUpdated = new();


        /// <summary>
        ///     Fired when a user's entitlement is deleted.
        /// </summary>
        public event Func<Cacheable<SocketEntitlement, ulong>, Task> EntitlementDeleted
        {
            add { _entitlementDeleted.Add(value); }
            remove { _entitlementDeleted.Remove(value); }
        }

        internal readonly AsyncEvent<Func<Cacheable<SocketEntitlement, ulong>, Task>> _entitlementDeleted = new();


        #endregion
    }
}
