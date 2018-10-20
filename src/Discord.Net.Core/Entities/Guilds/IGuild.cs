using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic guild/server.
    /// </summary>
    public interface IGuild : IDeletable, ISnowflakeEntity
    {
        /// <summary>
        ///     Gets the name of this guild.
        /// </summary>
        /// <returns>
        ///     A string containing the name of this guild.
        /// </returns>
        string Name { get; }
        /// <summary>
        ///     Gets the amount of time (in seconds) a user must be inactive in a voice channel for until they are
        ///     automatically moved to the AFK voice channel.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the amount of time in seconds for a user to be marked as inactive
        ///     and moved into the AFK voice channel.
        /// </returns>
        int AFKTimeout { get; }
        /// <summary>
        ///     Gets a value that indicates whether this guild is embeddable (i.e. can use widget).
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this guild can be embedded via widgets; otherwise <c>false</c>.
        /// </returns>
        bool IsEmbeddable { get; }
        /// <summary>
        ///     Gets the default message notifications for users who haven't explicitly set their notification settings.
        /// </summary>
        DefaultMessageNotifications DefaultMessageNotifications { get; }
        /// <summary>
        ///     Gets the level of Multi-Factor Authentication requirements a user must fulfill before being allowed to
        ///     perform administrative actions in this guild.
        /// </summary>
        /// <returns>
        ///     The level of MFA requirement.
        /// </returns>
        MfaLevel MfaLevel { get; }
        /// <summary>
        ///     Gets the level of requirements a user must fulfill before being allowed to post messages in this guild.
        /// </summary>
        /// <returns>
        ///     The level of requirements.
        /// </returns>
        VerificationLevel VerificationLevel { get; }
        /// <summary>
        ///     Gets the level of content filtering applied to user's content in a Guild.
        /// </summary>
        /// <returns>
        ///     The level of explicit content filtering.
        /// </returns>
        ExplicitContentFilterLevel ExplicitContentFilter { get; }
        /// <summary>
        ///     Gets the ID of this guild's icon.
        /// </summary>
        /// <returns>
        ///     An identifier for the splash image; <c>null</c> if none is set.
        /// </returns>
        string IconId { get; }
        /// <summary>
        ///     Gets the URL of this guild's icon.
        /// </summary>
        /// <returns>
        ///     A URL pointing to the guild's icon; <c>null</c> if none is set.
        /// </returns>
        string IconUrl { get; }
        /// <summary>
        ///     Gets the ID of this guild's splash image.
        /// </summary>
        /// <returns>
        ///     An identifier for the splash image; <c>null</c> if none is set.
        /// </returns>
        string SplashId { get; }
        /// <summary>
        ///     Gets the URL of this guild's splash image.
        /// </summary>
        /// <returns>
        ///     A URL pointing to the guild's splash image; <c>null</c> if none is set.
        /// </returns>
        string SplashUrl { get; }
        /// <summary>
        ///     Determines if this guild is currently connected and ready to be used.
        /// </summary>
        /// <remarks>
        ///     <note>
        ///         This property only applies to a WebSocket-based client.
        ///     </note>
        ///     This boolean is used to determine if the guild is currently connected to the WebSocket and is ready to be used/accessed.
        /// </remarks>
        /// <returns>
        ///     <c>true</c> if this guild is currently connected and ready to be used; otherwise <c>false</c>.
        /// </returns>
        bool Available { get; }

        /// <summary>
        ///     Gets the ID of the AFK voice channel for this guild.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the AFK voice channel; <c>null</c> if
        ///     none is set.
        /// </returns>
        ulong? AFKChannelId { get; }
        /// <summary>
        ///     Gets the ID of the default channel for this guild.
        /// </summary>
        /// <remarks>
        ///     This property retrieves the snowflake identifier of the first viewable text channel for this guild.
        ///     <note type="warning">
        ///         This channel does not guarantee the user can send message to it, as it only looks for the first viewable
        ///         text channel.
        ///     </note>
        /// </remarks>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the default text channel; <c>0</c> if
        ///     none can be found.
        /// </returns>
        ulong DefaultChannelId { get; }
        /// <summary>
        ///     Gets the ID of the widget embed channel of this guild.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the embedded channel found within the
        ///     widget settings of this guild; <c>null</c> if none is set.
        /// </returns>
        ulong? EmbedChannelId { get; }
        /// <summary>
        ///     Gets the ID of the channel where randomized welcome messages are sent.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the system channel where randomized
        ///     welcome messages are sent; <c>null</c> if none is set.
        /// </returns>
        ulong? SystemChannelId { get; }
        /// <summary>
        ///     Gets the ID of the user that owns this guild.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the user that owns this guild.
        /// </returns>
        ulong OwnerId { get; }
        /// <summary>
        ///     Gets the application ID of the guild creator if it is bot-created.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the application ID that created this guild, or <c>null</c> if it was not bot-created.
        /// </returns>
        ulong? ApplicationId { get; }
        /// <summary>
        ///     Gets the ID of the region hosting this guild's voice channels.
        /// </summary>
        /// <returns>
        ///     A string containing the identifier for the voice region that this guild uses (e.g. <c>eu-central</c>).
        /// </returns>
        string VoiceRegionId { get; }
        /// <summary>
        ///     Gets the <see cref="IAudioClient"/> currently associated with this guild.
        /// </summary>
        /// <returns>
        ///     An <see cref="IAudioClient"/> currently associated with this guild.
        /// </returns>
        IAudioClient AudioClient { get; }
        /// <summary>
        ///     Gets the built-in role containing all users in this guild.
        /// </summary>
        /// <returns>
        ///     A role object that represents an <c>@everyone</c> role in this guild.
        /// </returns>
        IRole EveryoneRole { get; }
        /// <summary>
        ///     Gets a collection of all custom emotes for this guild.
        /// </summary>
        /// <returns>
        ///     A read-only collection of all custom emotes for this guild.
        /// </returns>
        IReadOnlyCollection<GuildEmote> Emotes { get; }
        /// <summary>
        ///     Gets a collection of all extra features added to this guild.
        /// </summary>
        /// <returns>
        ///     A read-only collection of enabled features in this guild.
        /// </returns>
        IReadOnlyCollection<string> Features { get; }
        /// <summary>
        ///     Gets a collection of all roles in this guild.
        /// </summary>
        /// <returns>
        ///     A read-only collection of roles found within this guild.
        /// </returns>
        IReadOnlyCollection<IRole> Roles { get; }

        /// <summary>
        ///     Modifies this guild.
        /// </summary>
        /// <param name="func">The delegate containing the properties to modify the guild with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        Task ModifyAsync(Action<GuildProperties> func, RequestOptions options = null);
        /// <summary>
        ///     Modifies this guild's embed channel.
        /// </summary>
        /// <param name="func">The delegate containing the properties to modify the guild widget with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        Task ModifyEmbedAsync(Action<GuildEmbedProperties> func, RequestOptions options = null);
        /// <summary>
        ///     Bulk-modifies the order of channels in this guild.
        /// </summary>
        /// <param name="args">The properties used to modify the channel positions with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous reorder operation.
        /// </returns>
        Task ReorderChannelsAsync(IEnumerable<ReorderChannelProperties> args, RequestOptions options = null);
        /// <summary>
        ///     Bulk-modifies the order of roles in this guild.
        /// </summary>
        /// <param name="args">The properties used to modify the role positions with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous reorder operation.
        /// </returns>
        Task ReorderRolesAsync(IEnumerable<ReorderRoleProperties> args, RequestOptions options = null);
        /// <summary>
        ///     Leaves this guild.
        /// </summary>
        /// <remarks>
        ///     This method will make the currently logged-in user leave the guild.
        ///     <note>
        ///         If the user is the owner of this guild, use <see cref="IDeletable.DeleteAsync"/> instead.
        ///     </note>
        /// </remarks>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous leave operation.
        /// </returns>
        Task LeaveAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all users banned in this guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
        ///     ban objects that this guild currently possesses, with each object containing the user banned and reason
        ///     behind the ban.
        /// </returns>
        Task<IReadOnlyCollection<IBan>> GetBansAsync(RequestOptions options = null);
        /// <summary>
        ///     Gets a ban object for a banned user.
        /// </summary>
        /// <param name="user">The banned user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a ban object, which
        ///     contains the user information and the reason for the ban; <c>null</c> if the ban entry cannot be found.
        /// </returns>
        Task<IBan> GetBanAsync(IUser user, RequestOptions options = null);
        /// <summary>
        ///     Gets a ban object for a banned user.
        /// </summary>
        /// <param name="userId">The snowflake identifier for the banned user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a ban object, which
        ///     contains the user information and the reason for the ban; <c>null</c> if the ban entry cannot be found.
        /// </returns>
        Task<IBan> GetBanAsync(ulong userId, RequestOptions options = null);
        /// <summary>
        ///     Bans the user from this guild and optionally prunes their recent messages.
        /// </summary>
        /// <param name="user">The user to ban.</param>
        /// <param name="pruneDays">The number of days to remove messages from this user for, and this number must be between [0, 7].</param>
        /// <param name="reason">The reason of the ban to be written in the audit log.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="ArgumentException"><paramref name="pruneDays"/> is not between 0 to 7.</exception>
        /// <returns>
        ///     A task that represents the asynchronous add operation for the ban.
        /// </returns>
        Task AddBanAsync(IUser user, int pruneDays = 0, string reason = null, RequestOptions options = null);
        /// <summary>
        ///     Bans the user from this guild and optionally prunes their recent messages.
        /// </summary>
        /// <param name="userId">The snowflake ID of the user to ban.</param>
        /// <param name="pruneDays">The number of days to remove messages from this user for, and this number must be between [0, 7].</param>
        /// <param name="reason">The reason of the ban to be written in the audit log.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="ArgumentException"><paramref name="pruneDays"/> is not between 0 to 7.</exception>
        /// <returns>
        ///     A task that represents the asynchronous add operation for the ban.
        /// </returns>
        Task AddBanAsync(ulong userId, int pruneDays = 0, string reason = null, RequestOptions options = null);
        /// <summary>
        ///     Unbans the user if they are currently banned.
        /// </summary>
        /// <param name="user">The user to be unbanned.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous removal operation for the ban.
        /// </returns>
        Task RemoveBanAsync(IUser user, RequestOptions options = null);
        /// <summary>
        ///     Unbans the user if they are currently banned.
        /// </summary>
        /// <param name="userId">The snowflake identifier of the user to be unbanned.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous removal operation for the ban.
        /// </returns>
        Task RemoveBanAsync(ulong userId, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all channels in this guild.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
        ///     generic channels found within this guild.
        /// </returns>
        Task<IReadOnlyCollection<IGuildChannel>> GetChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a channel in this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the channel.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the generic channel
        ///     associated with the specified <paramref name="id"/>; <c>null</c> if none is found.
        /// </returns>
        Task<IGuildChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of all text channels in this guild.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
        ///     message channels found within this guild.
        /// </returns>
        Task<IReadOnlyCollection<ITextChannel>> GetTextChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a text channel in this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the text channel.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the text channel
        ///     associated with the specified <paramref name="id"/>; <c>null</c> if none is found.
        /// </returns>
        Task<ITextChannel> GetTextChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of all voice channels in this guild.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
        ///     voice channels found within this guild.
        /// </returns>
        Task<IReadOnlyCollection<IVoiceChannel>> GetVoiceChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of all category channels in this guild.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
        ///     category channels found within this guild.
        /// </returns>
        Task<IReadOnlyCollection<ICategoryChannel>> GetCategoriesAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a voice channel in this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the voice channel.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the voice channel associated
        ///     with the specified <paramref name="id"/>; <c>null</c> if none is found.
        /// </returns>
        Task<IVoiceChannel> GetVoiceChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the AFK voice channel in this guild.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the voice channel that the
        ///     AFK users will be moved to after they have idled for too long; <c>null</c> if none is set.
        /// </returns>
        Task<IVoiceChannel> GetAFKChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the system channel where randomized welcome messages are sent in this guild.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the text channel where
        ///     randomized welcome messages will be sent to; <c>null</c> if none is set.
        /// </returns>
        Task<ITextChannel> GetSystemChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the first viewable text channel in this guild.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the first viewable text
        ///     channel in this guild; <c>null</c> if none is found.
        /// </returns>
        Task<ITextChannel> GetDefaultChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the embed channel (i.e. the channel set in the guild's widget settings) in this guild.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the embed channel set
        ///     within the server's widget settings; <c>null</c> if none is set.
        /// </returns>
        Task<IGuildChannel> GetEmbedChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

        /// <summary>
        ///     Creates a new text channel in this guild.
        /// </summary>
        /// <example>
        ///     The following example creates a new text channel under an existing category named <c>Wumpus</c> with a set topic.
        ///     <code lang="cs">
        ///     var categories = await guild.GetCategoriesAsync();
        ///     var targetCategory = categories.FirstOrDefault(x => x.Name == "wumpus");
        ///     if (targetCategory == null) return;
        ///     await Context.Guild.CreateTextChannelAsync(name, x =>
        ///     {
        ///         x.CategoryId = targetCategory.Id;
        ///         x.Topic = $"This channel was created at {DateTimeOffset.UtcNow} by {user}.";
        ///     });
        ///     </code>
        /// </example>
        /// <param name="name">The new name for the text channel.</param>
        /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the newly created
        ///     text channel.
        /// </returns>
        Task<ITextChannel> CreateTextChannelAsync(string name, Action<TextChannelProperties> func = null, RequestOptions options = null);
        /// <summary>
        ///     Creates a new voice channel in this guild.
        /// </summary>
        /// <param name="name">The new name for the voice channel.</param>
        /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the newly created
        ///     voice channel.
        /// </returns>
        Task<IVoiceChannel> CreateVoiceChannelAsync(string name, Action<VoiceChannelProperties> func = null, RequestOptions options = null);
        /// <summary>
        ///     Creates a new channel category in this guild.
        /// </summary>
        /// <param name="name">The new name for the category.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the newly created
        ///     category channel.
        /// </returns>
        Task<ICategoryChannel> CreateCategoryAsync(string name, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all the voice regions this guild can access.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
        ///     voice regions the guild can access.
        /// </returns>
        Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegionsAsync(RequestOptions options = null);

        Task<IReadOnlyCollection<IGuildIntegration>> GetIntegrationsAsync(RequestOptions options = null);
        Task<IGuildIntegration> CreateIntegrationAsync(ulong id, string type, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all invites in this guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
        ///     invite metadata, each representing information for an invite found within this guild.
        /// </returns>
        Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions options = null);
        /// <summary>
        ///     Gets the vanity invite URL of this guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the partial metadata of
        ///     the vanity invite found within this guild; <c>null</c> if none is found.
        /// </returns>
        Task<IInviteMetadata> GetVanityInviteAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets a role in this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the role.</param>
        /// <returns>
        ///     A role that is associated with the specified <paramref name="id"/>; <c>null</c> if none is found.
        /// </returns>
        IRole GetRole(ulong id);
        /// <summary>
        ///     Creates a new role with the provided name.
        /// </summary>
        /// <param name="name">The new name for the role.</param>
        /// <param name="permissions">The guild permission that the role should possess.</param>
        /// <param name="color">The color of the role.</param>
        /// <param name="isHoisted">Whether the role is separated from others on the sidebar.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the newly created
        ///     role.
        /// </returns>
        Task<IRole> CreateRoleAsync(string name, GuildPermissions? permissions = null, Color? color = null, bool isHoisted = false, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all users in this guild.
        /// </summary>
        /// <remarks>
        ///     This method retrieves all users found within this guild.
        ///     <note>
        ///         This may return an incomplete collection in the WebSocket implementation due to how Discord does not
        ///         send a complete user list for large guilds.
        ///     </note>
        /// </remarks>
        /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a collection of guild
        ///     users found within this guild.
        /// </returns>
        Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a user from this guild.
        /// </summary>
        /// <remarks>
        ///     This method retrieves a user found within this guild.
        ///     <note>
        ///         This may return <c>null</c> in the WebSocket implementation due to incomplete user collection in
        ///         large guilds.
        ///     </note>
        /// </remarks>
        /// <param name="id">The snowflake identifier of the user.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the guild user
        ///     associated with the specified <paramref name="id"/>; <c>null</c> if none is found.
        /// </returns>
        Task<IGuildUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the current user for this guild.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the currently logged-in
        ///     user within this guild.
        /// </returns>
        Task<IGuildUser> GetCurrentUserAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the owner of this guild.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the owner of this guild.
        /// </returns>
        Task<IGuildUser> GetOwnerAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Downloads all users for this guild if the current list is incomplete.
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous download operation.
        /// </returns>
        Task DownloadUsersAsync();
        /// <summary>
        ///     Prunes inactive users.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method removes all users that have not logged on in the provided number of <paramref name="days"/>.
        ///     </para>
        ///     <para>
        ///         If <paramref name="simulate" /> is <c>true</c>, this method will only return the number of users that
        ///         would be removed without kicking the users.
        ///     </para>
        /// </remarks>
        /// <param name="days">The number of days required for the users to be kicked.</param>
        /// <param name="simulate">Whether this prune action is a simulation.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous prune operation. The task result contains the number of users to
        ///     be or has been removed from this guild.
        /// </returns>
        Task<int> PruneUsersAsync(int days = 30, bool simulate = false, RequestOptions options = null);

        /// <summary>
        ///     Gets the specified number of audit log entries for this guild.
        /// </summary>
        /// <param name="limit">The number of audit log entries to fetch.</param>
        /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
        ///     of the requested audit log entries.
        /// </returns>
        Task<IReadOnlyCollection<IAuditLogEntry>> GetAuditLogsAsync(int limit = DiscordConfig.MaxAuditLogEntriesPerBatch,
            CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

        /// <summary>
        ///     Gets a webhook found within this guild.
        /// </summary>
        /// <param name="id">The identifier for the webhook.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the webhook with the
        ///     specified <paramref name="id"/>; <c>null</c> if none is found.
        /// </returns>
        Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of all webhook from this guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
        ///     of webhooks found within the guild.
        /// </returns>
        Task<IReadOnlyCollection<IWebhook>> GetWebhooksAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets a specific emote from this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the guild emote.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the emote found with the
        ///     specified <paramref name="id"/>; <c>null</c> if none is found.
        /// </returns>
        Task<GuildEmote> GetEmoteAsync(ulong id, RequestOptions options = null);
        /// <summary>
        ///     Creates a new <see cref="GuildEmote"/> in this guild.
        /// </summary>
        /// <param name="name">The name of the guild emote.</param>
        /// <param name="image">The image of the new emote.</param>
        /// <param name="roles">The roles to limit the emote usage to.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the created emote.
        /// </returns>
        Task<GuildEmote> CreateEmoteAsync(string name, Image image, Optional<IEnumerable<IRole>> roles = default(Optional<IEnumerable<IRole>>), RequestOptions options = null);

        /// <summary>
        ///     Modifies an existing <see cref="GuildEmote"/> in this guild.
        /// </summary>
        /// <param name="emote">The emote to be modified.</param>
        /// <param name="func">The delegate containing the properties to modify the emote with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation. The task result contains the modified
        ///     emote.
        /// </returns>
        Task<GuildEmote> ModifyEmoteAsync(GuildEmote emote, Action<EmoteProperties> func, RequestOptions options = null);
        /// <summary>
        ///     Deletes an existing <see cref="GuildEmote"/> from this guild.
        /// </summary>
        /// <param name="emote">The emote to delete.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous removal operation.
        /// </returns>
        Task DeleteEmoteAsync(GuildEmote emote, RequestOptions options = null);
    }
}
