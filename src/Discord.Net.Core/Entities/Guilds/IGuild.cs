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
        string Name { get; }
        /// <summary>
        ///     Gets the amount of time (in seconds) a user must be inactive in a voice channel for until they are
        ///     automatically moved to the AFK voice channel, if one is set.
        /// </summary>
        int AFKTimeout { get; }
        /// <summary>
        ///     Determines if this guild is embeddable (i.e. can use widget).
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if this guild can be embedded via widgets; otherwise <see langword="false"/>.
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
        MfaLevel MfaLevel { get; }
        /// <summary>
        ///     Gets the level of requirements a user must fulfill before being allowed to post messages in this guild.
        /// </summary>
        VerificationLevel VerificationLevel { get; }
        /// <summary>
        ///     Returns the ID of this guild's icon, or <c>null</c> if none is set.
        /// </summary>
        string IconId { get; }
        /// <summary>
        ///     Returns the URL of this guild's icon, or <c>null</c> if none is set.
        /// </summary>
        string IconUrl { get; }
        /// <summary>
        ///     Returns the ID of this guild's splash image, or <c>null</c> if none is set.
        /// </summary>
        string SplashId { get; }
        /// <summary>
        ///     Returns the URL of this guild's splash image, or <c>null</c> if none is set.
        /// </summary>
        string SplashUrl { get; }
        /// <summary>
        ///     Determines if this guild is currently connected and ready to be used.
        /// </summary>
        /// <returns>
        ///     Returns <see langword="true"/> if this guild is currently connected and ready to be used. Only applies
        ///     to the WebSocket client.
        /// </returns>
        bool Available { get; }

        /// <summary>
        ///     Gets the ID of the AFK voice channel for this guild, or <c>null</c> if none is set.
        /// </summary>
        ulong? AFKChannelId { get; }
        /// <summary>
        ///     Gets the ID of the the default channel for this guild.
        /// </summary>
        ulong DefaultChannelId { get; }
        /// <summary>
        ///     Gets the ID of the embed channel set in the widget settings of this guild, or <c>null</c> if none is set.
        /// </summary>
        ulong? EmbedChannelId { get; }
        /// <summary>
        ///     Gets the ID of the channel where randomized welcome messages are sent, or <c>null</c> if none is set.
        /// </summary>
        ulong? SystemChannelId { get; }
        /// <summary>
        ///     Gets the ID of the user that created this guild.
        /// </summary>
        ulong OwnerId { get; }
        /// <summary>
        ///     Gets the ID of the region hosting this guild's voice channels.
        /// </summary>
        string VoiceRegionId { get; }
        /// <summary>
        ///     Gets the <see cref="IAudioClient" /> currently associated with this guild.
        /// </summary>
        /// <returns>
        ///     <see cref="IAudioClient" /> currently associated with this guild.
        /// </returns>
        IAudioClient AudioClient { get; }
        /// <summary>
        ///     Gets the built-in role containing all users in this guild.
        /// </summary>
        /// <returns>
        ///     Built-in role that represents an @everyone role in this guild.
        /// </returns>
        IRole EveryoneRole { get; }
        /// <summary>
        ///     Gets a collection of all custom emotes for this guild.
        /// </summary>
        /// <returns>
        ///     A collection of all custom emotes for this guild.
        /// </returns>
        IReadOnlyCollection<GuildEmote> Emotes { get; }
        /// <summary>
        ///     Gets a collection of all extra features added to this guild.
        /// </summary>
        /// <returns>
        ///     A collection of enabled features in this guild.
        /// </returns>
        IReadOnlyCollection<string> Features { get; }
        /// <summary>
        ///     Gets a collection of all roles in this guild.
        /// </summary>
        /// <returns>
        ///     A collection of roles found within this guild.
        /// </returns>
        IReadOnlyCollection<IRole> Roles { get; }

        /// <summary>
        ///     Modifies this guild.
        /// </summary>
        /// <param name="func">The delegate containing the properties to modify the guild with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task ModifyAsync(Action<GuildProperties> func, RequestOptions options = null);
        /// <summary>
        ///     Modifies this guild's embed channel.
        /// </summary>
        /// <param name="func">The delegate containing the properties to modify the guild widget with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task ModifyEmbedAsync(Action<GuildEmbedProperties> func, RequestOptions options = null);
        /// <summary>
        ///     Bulk modifies the order of channels in this guild.
        /// </summary>
        /// <param name="args">The properties used to modify the channel positions with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task ReorderChannelsAsync(IEnumerable<ReorderChannelProperties> args, RequestOptions options = null);
        /// <summary>
        ///     Bulk modifies the order of roles in this guild.
        /// </summary>
        /// <param name="args">The properties used to modify the role positions with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        Task ReorderRolesAsync(IEnumerable<ReorderRoleProperties> args, RequestOptions options = null);
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        /// <summary>
        ///     Leaves this guild. If you are the owner, use <see cref="IDeletable.DeleteAsync" /> instead.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task LeaveAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all users banned on this guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing a collection of banned users with reasons.
        /// </returns>
        Task<IReadOnlyCollection<IBan>> GetBansAsync(RequestOptions options = null);
        /// <summary>
        ///     Bans the provided user from this guild and optionally prunes their recent messages.
        /// </summary>
        /// <param name="user">The user to ban.</param>
        /// <param name="pruneDays">
        /// The number of days to remove messages from this <paramref name="user" /> for - must be between [0, 7].
        /// </param>
        /// <param name="reason">The reason of the ban to be written in the audit log.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="ArgumentException"><paramref name="pruneDays" /> is not between 0 to 7.</exception>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task AddBanAsync(IUser user, int pruneDays = 0, string reason = null, RequestOptions options = null);
        /// <summary>
        ///     Bans the provided user ID from this guild and optionally prunes their recent messages.
        /// </summary>
        /// <param name="userId">The ID of the user to ban.</param>
        /// <param name="pruneDays">
        /// The number of days to remove messages from this user for - must be between [0, 7].
        /// </param>
        /// <param name="reason">The reason of the ban to be written in the audit log.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="ArgumentException"><paramref name="pruneDays" /> is not between 0 to 7.</exception>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task AddBanAsync(ulong userId, int pruneDays = 0, string reason = null, RequestOptions options = null);
        /// <summary>
        ///     Unbans the provided user if they are currently banned.
        /// </summary>
        /// <param name="user">The user to be unbanned.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task RemoveBanAsync(IUser user, RequestOptions options = null);
        /// <summary>
        ///     Unbans the provided user ID if it is currently banned.
        /// </summary>
        /// <param name="userId">The snowflake ID of the user to be unbanned.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task RemoveBanAsync(ulong userId, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all channels in this guild.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing a collection of generic channels found within this guild.
        /// </returns>
        Task<IReadOnlyCollection<IGuildChannel>> GetChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the channel in this guild with the provided ID.
        /// </summary>
        /// <param name="id">The channel ID.</param>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the generic channel with the specified ID, or 
        ///     <c>null</c> if none is found.
        /// </returns>
        Task<IGuildChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of all text channels in this guild.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing a collection of text channels found within this guild.
        /// </returns>
        Task<IReadOnlyCollection<ITextChannel>> GetTextChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a text channel in this guild with the provided ID, or <see langword="null" /> if not found.
        /// </summary>
        /// <param name="id">The text channel ID.</param>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the text channel with the specified ID, or 
        ///     <c>null</c> if none is found.
        /// </returns>
        Task<ITextChannel> GetTextChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of all voice channels in this guild.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing a collection of voice channels found within this guild.
        /// </returns>
        Task<IReadOnlyCollection<IVoiceChannel>> GetVoiceChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of all category channels in this guild.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing a collection of category channels found within this guild.
        /// </returns>
        Task<IReadOnlyCollection<ICategoryChannel>> GetCategoriesAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the voice channel in this guild with the provided ID.
        /// </summary>
        /// <param name="id">The text channel ID.</param>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the voice channel with the specified ID, or 
        ///     <c>null</c> if none is found.
        /// </returns>
        Task<IVoiceChannel> GetVoiceChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the AFK voice channel in this guild.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the AFK voice channel set within this guild, or 
        ///     <c>null</c> if none is set.
        /// </returns>
        Task<IVoiceChannel> GetAFKChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the default system text channel in this guild with the provided ID.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the system channel within this guild, or 
        ///     <see langword="null" /> if none is set.
        /// </returns>
        Task<ITextChannel> GetSystemChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the top viewable text channel in this guild with the provided ID.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the first viewable text channel in this guild, or 
        ///     <c>null</c> if none is found.
        /// </returns>
        Task<ITextChannel> GetDefaultChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the embed channel (i.e. the channel set in the guild's widget settings) in this guild.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the embed channel set within the server's widget settings, or
        ///     <c>null</c> if none is set.
        /// </returns>
        Task<IGuildChannel> GetEmbedChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Creates a new text channel.
        /// </summary>
        /// <param name="name">The new name for the text channel.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the newly created text channel.
        /// </returns>
        Task<ITextChannel> CreateTextChannelAsync(string name, RequestOptions options = null);
        /// <summary>
        ///     Creates a new voice channel.
        /// </summary>
        /// <param name="name">The new name for the voice channel.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the newly created voice channel.
        /// </returns>
        Task<IVoiceChannel> CreateVoiceChannelAsync(string name, RequestOptions options = null);
        /// <summary>
        ///     Creates a new channel category.
        /// </summary>
        /// <param name="name">The new name for the category.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the newly created category channel.
        /// </returns>
        Task<ICategoryChannel> CreateCategoryAsync(string name, RequestOptions options = null);

        Task<IReadOnlyCollection<IGuildIntegration>> GetIntegrationsAsync(RequestOptions options = null);
        Task<IGuildIntegration> CreateIntegrationAsync(ulong id, string type, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all invites to this guild.
        /// </summary>
        Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets the role in this guild with the provided ID, or <c>null</c> if not found.
        /// </summary>
        /// <param name="id">The role ID.</param>
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
        ///     An awaitable <see cref="Task"/> containing the newly crated role.
        /// </returns>
        Task<IRole> CreateRoleAsync(string name, GuildPermissions? permissions = null, Color? color = null, bool isHoisted = false, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all users in this guild.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from
        /// cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing a collection of users found within this guild.
        /// </returns>
        /// <remarks>
        ///     This may return an incomplete list on the WebSocket implementation because Discord only sends offline
        ///     users on large guilds.
        /// </remarks>
        Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the user in this guild with the provided ID, or <c>null</c> if not found.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the guild user with the specified ID, otherwise <c>null</c>.
        /// </returns>
        Task<IGuildUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the current user for this guild.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the currently logged-in user within this guild.
        /// </returns>
        Task<IGuildUser> GetCurrentUserAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the owner of this guild.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the owner of this guild.
        /// </returns>
        Task<IGuildUser> GetOwnerAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Downloads all users for this guild if the current list is incomplete.
        /// </summary>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task DownloadUsersAsync();
        /// <summary>
        ///     Removes all users from this guild if they have not logged on in a provided number of
        ///     <paramref name="days" /> or, if <paramref name="simulate" /> is <see langword="true"/>, returns the
        ///     number of users that would be removed.
        /// </summary>
        /// <param name="days">The number of days required for the users to be kicked.</param>
        /// <param name="simulate">Whether this prune action is a simulation.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the number of users to be or has been removed from this guild.
        /// </returns>
        Task<int> PruneUsersAsync(int days = 30, bool simulate = false, RequestOptions options = null);

        /// <summary>
        ///     Gets the webhook in this guild with the provided ID, or <c>null</c> if not found.
        /// </summary>
        /// <param name="id">The webhook ID.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the webhook with the specified ID, otherwise <c>null</c>.
        /// </returns>
        Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of all webhook from this guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing a collection of webhooks found within the guild.
        /// </returns>
        Task<IReadOnlyCollection<IWebhook>> GetWebhooksAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets a specific emote from this guild.
        /// </summary>
        /// <param name="id">The guild emote ID.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the emote found with the specified ID, or <c>null</c> if not found.
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
        ///     An awaitable <see cref="Task"/> containing the created emote.
        /// </returns>
        Task<GuildEmote> CreateEmoteAsync(string name, Image image, Optional<IEnumerable<IRole>> roles = default(Optional<IEnumerable<IRole>>), RequestOptions options = null);

        /// <summary>
        ///     Modifies an existing <see cref="GuildEmote"/> in this guild.
        /// </summary>
        /// <param name="emote">The emote to be modified.</param>
        /// <param name="func">The delegate containing the properties to modify the emote with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the newly modified emote.
        /// </returns>
        Task<GuildEmote> ModifyEmoteAsync(GuildEmote emote, Action<EmoteProperties> func, RequestOptions options = null);
        /// <summary>
        ///     Deletes an existing <see cref="GuildEmote"/> from this guild.
        /// </summary>
        /// <param name="emote">The emote to delete.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task DeleteEmoteAsync(GuildEmote emote, RequestOptions options = null);
    }
}
