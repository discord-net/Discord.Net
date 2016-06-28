using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.API.Rest;
using Discord.Audio;

namespace Discord
{
    public interface IGuild : IDeletable, ISnowflakeEntity, IUpdateable
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
        /// <summary> Returns the url to this guild's icon, or null if one is not set. </summary>
        string IconUrl { get; }
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
        IReadOnlyCollection<Emoji> Emojis { get; }
        /// <summary> Gets a collection of all extra features added to this guild. </summary>
        IReadOnlyCollection<string> Features { get; }
        /// <summary> Gets a collection of all roles in this guild. </summary>
        IReadOnlyCollection<IRole> Roles { get; }

        /// <summary> Modifies this guild. </summary>
        Task ModifyAsync(Action<ModifyGuildParams> func);
        /// <summary> Modifies this guild's embed. </summary>
        Task ModifyEmbedAsync(Action<ModifyGuildEmbedParams> func);
        /// <summary> Bulk modifies the channels of this guild. </summary>
        Task ModifyChannelsAsync(IEnumerable<ModifyGuildChannelsParams> args);
        /// <summary> Bulk modifies the roles of this guild. </summary>
        Task ModifyRolesAsync(IEnumerable<ModifyGuildRolesParams> args);
        /// <summary> Leaves this guild. If you are the owner, use Delete instead. </summary>
        Task LeaveAsync();

        /// <summary> Gets a collection of all users banned on this guild. </summary>
        Task<IReadOnlyCollection<IUser>> GetBansAsync();
        /// <summary> Bans the provided user from this guild and optionally prunes their recent messages. </summary>
        Task AddBanAsync(IUser user, int pruneDays = 0);
        /// <summary> Bans the provided user id from this guild and optionally prunes their recent messages. </summary>
        Task AddBanAsync(ulong userId, int pruneDays = 0);
        /// <summary> Unbans the provided user if it is currently banned. </summary>
        Task RemoveBanAsync(IUser user);
        /// <summary> Unbans the provided user id if it is currently banned. </summary>
        Task RemoveBanAsync(ulong userId);

        /// <summary> Gets a collection of all channels in this guild. </summary>
        Task<IReadOnlyCollection<IGuildChannel>> GetChannelsAsync();
        /// <summary> Gets the channel in this guild with the provided id, or null if not found. </summary>
        Task<IGuildChannel> GetChannelAsync(ulong id);
        /// <summary> Creates a new text channel. </summary>
        Task<ITextChannel> CreateTextChannelAsync(string name);
        /// <summary> Creates a new voice channel. </summary>
        Task<IVoiceChannel> CreateVoiceChannelAsync(string name);

        /// <summary> Gets a collection of all invites to this guild. </summary>
        Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync();
        /// <summary> Creates a new invite to this guild. </summary>
        /// <param name="maxAge"> The time (in seconds) until the invite expires. Set to null to never expire. </param>
        /// <param name="maxUses"> The max amount  of times this invite may be used. Set to null to have unlimited uses. </param>
        /// <param name="isTemporary"> If true, a user accepting this invite will be kicked from the guild after closing their client. </param>
        /// <param name="withXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to null. </param>
        Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 1800, int? maxUses = default(int?), bool isTemporary = false, bool withXkcd = false);

        /// <summary> Gets the role in this guild with the provided id, or null if not found. </summary>
        IRole GetRole(ulong id);
        /// <summary> Creates a new role. </summary>
        Task<IRole> CreateRoleAsync(string name, GuildPermissions? permissions = null, Color? color = null, bool isHoisted = false);

        /// <summary> Gets a collection of all users in this guild. </summary>
        Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync();
        /// <summary> Gets the user in this guild with the provided id, or null if not found. </summary>
        Task<IGuildUser> GetUserAsync(ulong id);
        /// <summary> Gets the current user for this guild. </summary>
        Task<IGuildUser> GetCurrentUserAsync();
        /// <summary> Downloads all users for this guild if the current list is incomplete. Only applies to the WebSocket client. </summary>
        Task DownloadUsersAsync();
        /// <summary> Removes all users from this guild if they have not logged on in a provided number of days or, if simulate is true, returns the number of users that would be removed. </summary>
        Task<int> PruneUsersAsync(int days = 30, bool simulate = false);
    }
}