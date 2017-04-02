using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public interface IGuild : IDeletable, ISnowflakeEntity
    {
        /// <summary> Gets the name of this guild. </summary>
        string Name { get; }
        /// <summary> Gets the amount of time (in seconds) a user must be inactive in a voice channel for until they are automatically moved to the AFK voice channel, if one is set. </summary>
        int AFKTimeout { get; }
        /// <summary> Returns true if this guild is embeddable (e.g. widget) </summary>
        bool IsEmbeddable { get; }
        /// <summary> Gets the default message notifications for users who haven't explicitly set their notification settings. </summary>
        DefaultMessageNotifications DefaultMessageNotifications { get; }
        /// <summary> Gets the level of mfa requirements a user must fulfill before being allowed to perform administrative actions in this guild. </summary>
        MfaLevel MfaLevel { get; }
        /// <summary> Gets the level of requirements a user must fulfill before being allowed to post messages in this guild. </summary>
        VerificationLevel VerificationLevel { get; }
        /// <summary> Returns the id of this guild's icon, or null if one is not set. </summary>
        string IconId { get; }
        /// <summary> Returns the url to this guild's icon, or null if one is not set. </summary>
        string IconUrl { get; }
        /// <summary> Returns the id of this guild's splash image, or null if one is not set. </summary>
        string SplashId { get; }
        /// <summary> Returns the url to this guild's splash image, or null if one is not set. </summary>
        string SplashUrl { get; }
        /// <summary> Returns true if this guild is currently connected and ready to be used. Only applies to the WebSocket client. </summary>
        bool Available { get; }

        /// <summary> Gets the id of the AFK voice channel for this guild if set, or null if not. </summary>
        ulong? AFKChannelId { get; }
        /// <summary> Gets the id of the the default channel for this guild. </summary>
        ulong DefaultChannelId { get; }
        /// <summary> Gets the id of the embed channel for this guild if set, or null if not. </summary>
        ulong? EmbedChannelId { get; }
        /// <summary> Gets the id of the user that created this guild. </summary>
        ulong OwnerId { get; }
        /// <summary> Gets the id of the region hosting this guild's voice channels. </summary>
        string VoiceRegionId { get; }
        /// <summary> Gets the IAudioClient currently associated with this guild. </summary>
        IAudioClient AudioClient { get; }
        /// <summary> Gets the built-in role containing all users in this guild. </summary>
        IRole EveryoneRole { get; }
        /// <summary> Gets a collection of all custom emojis for this guild. </summary>
        IReadOnlyCollection<GuildEmoji> Emojis { get; }
        /// <summary> Gets a collection of all extra features added to this guild. </summary>
        IReadOnlyCollection<string> Features { get; }
        /// <summary> Gets a collection of all roles in this guild. </summary>
        IReadOnlyCollection<IRole> Roles { get; }

        /// <summary> Modifies this guild. </summary>
        Task ModifyAsync(Action<GuildProperties> func, RequestOptions options = null);
        /// <summary> Modifies this guild's embed. </summary>
        Task ModifyEmbedAsync(Action<GuildEmbedProperties> func, RequestOptions options = null);
        /// <summary> Bulk modifies the channels of this guild. </summary>
        Task ReorderChannelsAsync(IEnumerable<ReorderChannelProperties> args, RequestOptions options = null);
        /// <summary> Bulk modifies the roles of this guild. </summary>
        Task ReorderRolesAsync(IEnumerable<ReorderRoleProperties> args, RequestOptions options = null);
        /// <summary> Leaves this guild. If you are the owner, use Delete instead. </summary>
        Task LeaveAsync(RequestOptions options = null);

        /// <summary> Gets a collection of all users banned on this guild. </summary>
        Task<IReadOnlyCollection<IBan>> GetBansAsync(RequestOptions options = null);
        /// <summary> Bans the provided user from this guild and optionally prunes their recent messages. </summary>
        Task AddBanAsync(IUser user, int pruneDays = 0, RequestOptions options = null);
        /// <summary> Bans the provided user id from this guild and optionally prunes their recent messages. </summary>
        Task AddBanAsync(ulong userId, int pruneDays = 0, RequestOptions options = null);
        /// <summary> Unbans the provided user if it is currently banned. </summary>
        Task RemoveBanAsync(IUser user, RequestOptions options = null);
        /// <summary> Unbans the provided user id if it is currently banned. </summary>
        Task RemoveBanAsync(ulong userId, RequestOptions options = null);

        /// <summary> Gets a collection of all channels in this guild. </summary>
        Task<IReadOnlyCollection<IGuildChannel>> GetChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary> Gets the channel in this guild with the provided id, or null if not found. </summary>
        Task<IGuildChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<IReadOnlyCollection<ITextChannel>> GetTextChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<ITextChannel> GetTextChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<IReadOnlyCollection<IVoiceChannel>> GetVoiceChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<IVoiceChannel> GetVoiceChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<IVoiceChannel> GetAFKChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<ITextChannel> GetDefaultChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<IGuildChannel> GetEmbedChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary> Creates a new text channel. </summary>
        Task<ITextChannel> CreateTextChannelAsync(string name, RequestOptions options = null);
        /// <summary> Creates a new voice channel. </summary>
        Task<IVoiceChannel> CreateVoiceChannelAsync(string name, RequestOptions options = null);

        Task<IReadOnlyCollection<IGuildIntegration>> GetIntegrationsAsync(RequestOptions options = null);
        Task<IGuildIntegration> CreateIntegrationAsync(ulong id, string type, RequestOptions options = null);

        /// <summary> Gets a collection of all invites to this guild. </summary>
        Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions options = null);

        /// <summary> Gets the role in this guild with the provided id, or null if not found. </summary>
        IRole GetRole(ulong id);
        /// <summary> Creates a new role. </summary>
        Task<IRole> CreateRoleAsync(string name, GuildPermissions? permissions = null, Color? color = null, bool isHoisted = false, RequestOptions options = null);

        /// <summary> Gets a collection of all users in this guild. </summary>
        Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null); //TODO: shouldnt this be paged?
        /// <summary> Gets the user in this guild with the provided id, or null if not found. </summary>
        Task<IGuildUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary> Gets the current user for this guild. </summary>
        Task<IGuildUser> GetCurrentUserAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary> Gets the owner of this guild. </summary>
        Task<IGuildUser> GetOwnerAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary> Downloads all users for this guild if the current list is incomplete. </summary>
        Task DownloadUsersAsync();
        /// <summary> Removes all users from this guild if they have not logged on in a provided number of days or, if simulate is true, returns the number of users that would be removed. </summary>
        Task<int> PruneUsersAsync(int days = 30, bool simulate = false, RequestOptions options = null);
    }
}