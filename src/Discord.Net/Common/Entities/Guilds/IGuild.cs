using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.API.Rest;

namespace Discord
{
    public interface IGuild : IDeletable, ISnowflakeEntity, IUpdateable
    {
        /// <summary> Gets the amount of time (in seconds) a user must be inactive in a voice channel for until they are automatically moved to the AFK voice channel, if one is set. </summary>
        int AFKTimeout { get; }
        /// <summary> Returns true if this guild is embeddable (e.g. widget) </summary>
        bool IsEmbeddable { get; }
        /// <summary> Returns true if the current user owns this guild. </summary>
        bool IsOwner { get; }
        /// <summary> Gets the name of this guild. </summary>
        string Name { get; }
        int VerificationLevel { get; }

        /// <summary> Gets the id of the AFK voice channel for this guild if set, or null if not. </summary>
        ulong? AFKChannelId { get; }
        /// <summary> Gets the id of the the default channel for this guild. </summary>
        ulong DefaultChannelId { get; }
        /// <summary> Gets the id of the embed channel for this guild if set, or null if not. </summary>
        ulong? EmbedChannelId { get; }
        /// <summary> Gets the id of the role containing all users in this guild. </summary>
        ulong EveryoneRoleId { get; }
        /// <summary> Gets the id of the user that created this guild. </summary>
        ulong OwnerId { get; }
        /// <summary> Gets the id of the server region hosting this guild's voice channels. </summary>
        string VoiceRegionId { get; }

        /// <summary> Returns the url to this server's icon, or null if one is not set. </summary>
        string IconUrl { get; }
        /// <summary> Returns the url to this server's splash image, or null if one is not set. </summary>
        string SplashUrl { get; }

        /// <summary> Gets a collection of all custom emojis for this guild. </summary>
        IEnumerable<Emoji> Emojis { get; }
        /// <summary> Gets a collection of all extra features added to this guild. </summary>
        IEnumerable<string> Features { get; }

        /// <summary> Modifies this guild. </summary>
        Task Modify(Action<ModifyGuildParams> func);
        /// <summary> Modifies this guild's embed. </summary>
        Task ModifyEmbed(Action<ModifyGuildEmbedParams> func);
        /// <summary> Bulk modifies the channels of this guild. </summary>
        Task ModifyChannels(IEnumerable<ModifyGuildChannelsParams> args);
        /// <summary> Bulk modifies the roles of this guild. </summary>
        Task ModifyRoles(IEnumerable<ModifyGuildRolesParams> args);
        /// <summary> Leaves this guild. If you are the owner, use Delete instead. </summary>
        Task Leave();

        /// <summary> Gets a collection of all users banned on this guild. </summary>
        Task<IEnumerable<IUser>> GetBans();
        /// <summary> Bans the provided user from this guild and optionally prunes their recent messages. </summary>
        Task AddBan(IUser user, int pruneDays = 0);
        /// <summary> Bans the provided user id from this guild and optionally prunes their recent messages. </summary>
        Task AddBan(ulong userId, int pruneDays = 0);
        /// <summary> Unbans the provided user if it is currently banned. </summary>
        Task RemoveBan(IUser user);
        /// <summary> Unbans the provided user id if it is currently banned. </summary>
        Task RemoveBan(ulong userId);

        /// <summary> Gets a collection of all channels in this guild. </summary>
        Task<IEnumerable<IGuildChannel>> GetChannels();
        /// <summary> Gets the channel in this guild with the provided id, or null if not found. </summary>
        Task<IGuildChannel> GetChannel(ulong id);
        /// <summary> Creates a new text channel. </summary>
        Task<ITextChannel> CreateTextChannel(string name);
        /// <summary> Creates a new voice channel. </summary>
        Task<IVoiceChannel> CreateVoiceChannel(string name);

        /// <summary> Gets a collection of all invites to this guild. </summary>
        Task<IEnumerable<IGuildInvite>> GetInvites();
        /// <summary> Creates a new invite to this guild. </summary>
        /// <param name="maxAge"> The time (in seconds) until the invite expires. Set to null to never expire. </param>
        /// <param name="maxUses"> The max amount  of times this invite may be used. Set to null to have unlimited uses. </param>
        /// <param name="isTemporary"> If true, a user accepting this invite will be kicked from the guild after closing their client. </param>
        /// <param name="withXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to null. </param>
        Task<IGuildInvite> CreateInvite(int? maxAge = 1800, int? maxUses = default(int?), bool isTemporary = false, bool withXkcd = false);

        /// <summary> Gets a collection of all roles in this guild. </summary>
        Task<IEnumerable<IRole>> GetRoles();
        /// <summary> Gets the role in this guild with the provided id, or null if not found. </summary>
        Task<IRole> GetRole(ulong id);
        /// <summary> Creates a new role. </summary>
        Task<IRole> CreateRole(string name, GuildPermissions? permissions = null, Color? color = null, bool isHoisted = false);

        /// <summary> Gets a collection of all users in this guild. </summary>
        Task<IEnumerable<IGuildUser>> GetUsers();
        /// <summary> Gets the user in this guild with the provided id, or null if not found. </summary>
        Task<IGuildUser> GetUser(ulong id);     
        Task<int> PruneUsers(int days = 30, bool simulate = false);
    }
}