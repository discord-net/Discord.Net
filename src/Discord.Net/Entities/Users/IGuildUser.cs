using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.API.Rest;

namespace Discord
{
    /// <summary> A Guild-User pairing. </summary>
    public interface IGuildUser : IUpdateable, IUser, IVoiceState
    {
        /// <summary> Gets when this user joined this guild. </summary>
        DateTimeOffset? JoinedAt { get; }
        /// <summary> Gets the nickname for this user. </summary>
        string Nickname { get; }
        /// <summary> Gets the guild-level permissions granted to this user by their roles. </summary>
        GuildPermissions GuildPermissions { get; }

        /// <summary> Gets the guild for this guild-user pair. </summary>
        IGuild Guild { get; }
        /// <summary> Returns a collection of the roles this user is a member of in this guild, including the guild's @everyone role. </summary>
        IReadOnlyCollection<IRole> Roles { get; }

        /// <summary> Gets the level permissions granted to this user to a given channel. </summary>
        ChannelPermissions GetPermissions(IGuildChannel channel);

        /// <summary> Kicks this user from this guild. </summary>
        Task KickAsync();
        /// <summary> Modifies this user's properties in this guild. </summary>
        Task ModifyAsync(Action<ModifyGuildMemberParams> func);

        /// <summary> Returns a private message channel to this user, creating one if it does not already exist. </summary>
        Task<IDMChannel> CreateDMChannelAsync();
    }
}
