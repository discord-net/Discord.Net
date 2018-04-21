using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic guild object.
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
        ///     Returns <see langword="true"/> if this guild is embeddable (e.g. widget).
        /// </summary>
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
        ///     Returns the ID of this guild's icon, or <see langword="null"/> if one is not set.
        /// </summary>
        string IconId { get; }
        /// <summary>
        ///     Returns the URL of this guild's icon, or <see langword="null"/> if one is not set.
        /// </summary>
        string IconUrl { get; }
        /// <summary>
        ///     Returns the ID of this guild's splash image, or <see langword="null"/> if one is not set.
        /// </summary>
        string SplashId { get; }
        /// <summary>
        ///     Returns the URL of this guild's splash image, or <see langword="null"/> if one is not set.
        /// </summary>
        string SplashUrl { get; }
        /// <summary>
        ///     Returns <see langword="true"/> if this guild is currently connected and ready to be used. Only applies
        ///     to the WebSocket client.
        /// </summary>
        bool Available { get; }

        /// <summary>
        ///     Gets the ID of the AFK voice channel for this guild if set, or <see langword="null"/> if not.
        /// </summary>
        ulong? AFKChannelId { get; }
        /// <summary>
        ///     Gets the ID of the the default channel for this guild.
        /// </summary>
        ulong DefaultChannelId { get; }
        /// <summary>
        ///     Gets the ID of the embed channel for this guild if set, or <see langword="null"/> if not.
        /// </summary>
        ulong? EmbedChannelId { get; }
        /// <summary>
        ///     Gets the ID of the channel where randomized welcome messages are sent if set, or <see langword="null"/> if not.
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
        IAudioClient AudioClient { get; }
        /// <summary>
        ///     Gets the built-in role containing all users in this guild.
        /// </summary>
        IRole EveryoneRole { get; }
        /// <summary>
        ///     Gets a collection of all custom emotes for this guild.
        /// </summary>
        IReadOnlyCollection<GuildEmote> Emotes { get; }
        /// <summary>
        ///     Gets a collection of all extra features added to this guild.
        /// </summary>
        IReadOnlyCollection<string> Features { get; }
        /// <summary>
        ///     Gets a collection of all roles in this guild.
        /// </summary>
        IReadOnlyCollection<IRole> Roles { get; }

        /// <summary>
        ///     Modifies this guild.
        /// </summary>
        Task ModifyAsync(Action<GuildProperties> func, RequestOptions options = null);
        /// <summary>
        ///     Modifies this guild's embed channel.
        /// </summary>
        Task ModifyEmbedAsync(Action<GuildEmbedProperties> func, RequestOptions options = null);
        /// <summary>
        ///     Bulk modifies the order of channels in this guild.
        /// </summary>
        Task ReorderChannelsAsync(IEnumerable<ReorderChannelProperties> args, RequestOptions options = null);
        /// <summary>
        ///     Bulk modifies the order of roles in this guild.
        /// </summary>
        Task ReorderRolesAsync(IEnumerable<ReorderRoleProperties> args, RequestOptions options = null);
        /// <summary>
        ///     Leaves this guild. If you are the owner, use
        ///     <see cref="IDeletable.DeleteAsync(Discord.RequestOptions)" /> instead.
        /// </summary>
        Task LeaveAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all users banned on this guild.
        /// </summary>
        Task<IReadOnlyCollection<IBan>> GetBansAsync(RequestOptions options = null);
        /// <summary>
        ///     Bans the provided <paramref name="user"/> from this guild and optionally prunes their recent messages.
        /// </summary>
        /// <param name="user">
        ///     The user to ban.
        /// </param>
        /// <param name="pruneDays">
        ///     The number of days to remove messages from this <paramref name="user"/> for - must be between [0, 7]
        /// </param>
        /// <param name="reason">
        ///     The reason of the ban to be written in the audit log.
        /// </param>
        Task AddBanAsync(IUser user, int pruneDays = 0, string reason = null, RequestOptions options = null);
        /// <summary>
        ///     Bans the provided user ID from this guild and optionally prunes their recent messages.
        /// </summary>
        /// <param name="userId">
        ///     The ID of the user to ban.
        /// </param>
        /// <param name="pruneDays">
        ///     The number of days to remove messages from this user for - must be between [0, 7]
        /// </param>
        /// <param name="reason">
        ///     The reason of the ban to be written in the audit log.
        /// </param>
        Task AddBanAsync(ulong userId, int pruneDays = 0, string reason = null, RequestOptions options = null);
        /// <summary>
        ///     Unbans the provided <paramref name="user"/> if they are currently banned.
        /// </summary>
        Task RemoveBanAsync(IUser user, RequestOptions options = null);
        /// <summary>
        ///     Unbans the provided user ID if it is currently banned.
        /// </summary>
        Task RemoveBanAsync(ulong userId, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all channels in this guild.
        /// </summary>
        Task<IReadOnlyCollection<IGuildChannel>> GetChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the channel in this guild with the provided ID, or <see langword="null"/> if not found.
        /// </summary>
        /// <param name="id">The channel ID.</param>
        Task<IGuildChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of all text channels in this guild.
        /// </summary>
        Task<IReadOnlyCollection<ITextChannel>> GetTextChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a text channel in this guild with the provided ID, or <see langword="null"/> if not found.
        /// </summary>
        /// <param name="id">The text channel ID.</param>
        Task<ITextChannel> GetTextChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of all voice channels in this guild.
        /// </summary>
        Task<IReadOnlyCollection<IVoiceChannel>> GetVoiceChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of all category channels in this guild.
        /// </summary>
        Task<IReadOnlyCollection<ICategoryChannel>> GetCategoriesAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the voice channel in this guild with the provided ID, or <see langword="null"/> if not found.
        /// </summary>
        /// <param name="id">The text channel ID.</param>
        Task<IVoiceChannel> GetVoiceChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the voice AFK channel in this guild with the provided ID, or <see langword="null"/> if not found.
        /// </summary>
        Task<IVoiceChannel> GetAFKChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the default system text channel in this guild with the provided ID, or <see langword="null"/> if
        ///     none is set.
        /// </summary>
        Task<ITextChannel> GetSystemChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the top viewable text channel in this guild with the provided ID, or <see langword="null"/> if not
        ///     found.
        /// </summary>
        Task<ITextChannel> GetDefaultChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the embed channel in this guild.
        /// </summary>
        Task<IGuildChannel> GetEmbedChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Creates a new text channel.
        /// </summary>
        /// <param name="name">The new name for the text channel.</param>
        Task<ITextChannel> CreateTextChannelAsync(string name, RequestOptions options = null);
        /// <summary>
        ///     Creates a new voice channel.
        /// </summary>
        /// <param name="name">The new name for the voice channel.</param>
        Task<IVoiceChannel> CreateVoiceChannelAsync(string name, RequestOptions options = null);
        /// <summary>
        ///     Creates a new channel category.
        /// </summary>
        /// <param name="name">The new name for the category.</param>
        Task<ICategoryChannel> CreateCategoryAsync(string name, RequestOptions options = null);

        Task<IReadOnlyCollection<IGuildIntegration>> GetIntegrationsAsync(RequestOptions options = null);
        Task<IGuildIntegration> CreateIntegrationAsync(ulong id, string type, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all invites to this guild.
        /// </summary>
        Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets the role in this guild with the provided ID, or <see langword="null"/> if not found.
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
        Task<IRole> CreateRoleAsync(string name, GuildPermissions? permissions = null, Color? color = null, bool isHoisted = false, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all users in this guild.
        /// </summary>
        Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null); //TODO: shouldnt this be paged?
        /// <summary>
        ///     Gets the user in this guild with the provided ID, or <see langword="null"/> if not found.
        /// </summary>
        /// <param name="id">The user ID.</param>
        Task<IGuildUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the current user for this guild.
        /// </summary>
        Task<IGuildUser> GetCurrentUserAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets the owner of this guild.
        /// </summary>
        Task<IGuildUser> GetOwnerAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Downloads all users for this guild if the current list is incomplete.
        /// </summary>
        Task DownloadUsersAsync();
        /// <summary>
        ///     Removes all users from this guild if they have not logged on in a provided number of
        ///     <paramref name="days"/> or, if <paramref name="simulate"/> is true, returns the number of users that
        ///     would be removed.
        /// </summary>
        /// <param name="days">The number of days required for the users to be kicked.</param>
        /// <param name="simulate">Whether this prune action is a simulation.</param>
        /// <returns>
        ///     The number of users removed from this guild.
        /// </returns>
        Task<int> PruneUsersAsync(int days = 30, bool simulate = false, RequestOptions options = null);

        /// <summary>
        ///     Gets the webhook in this guild with the provided ID, or <see langword="null"/> if not found.
        /// </summary>
        /// <param name="id">The webhook ID.</param>
        Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of all webhooks from this guild.
        /// </summary>
        Task<IReadOnlyCollection<IWebhook>> GetWebhooksAsync(RequestOptions options = null);
        
        /// <summary>
        ///     Gets a specific emote from this guild.
        /// </summary>
        /// <param name="id">The guild emote ID.</param>
        Task<GuildEmote> GetEmoteAsync(ulong id, RequestOptions options = null);
        /// <summary>
        ///     Creates a new emote in this guild.
        /// </summary>
        /// <param name="name">The name of the guild emote.</param>
        /// <param name="image">The image of the new emote.</param>
        /// <param name="roles">The roles to limit the emote usage to.</param>
        Task<GuildEmote> CreateEmoteAsync(string name, Image image, Optional<IEnumerable<IRole>> roles = default(Optional<IEnumerable<IRole>>), RequestOptions options = null);
        /// <summary>
        ///     Modifies an existing <paramref name="emote"/> in this guild.
        /// </summary>
        Task<GuildEmote> ModifyEmoteAsync(GuildEmote emote, Action<EmoteProperties> func, RequestOptions options = null);
        /// <summary>
        ///     Deletes an existing <paramref name="emote"/> from this guild.
        /// </summary>
        /// <param name="emote">The guild emote to delete.</param>
        Task DeleteEmoteAsync(GuildEmote emote, RequestOptions options = null);
    }
}
