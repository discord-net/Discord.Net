using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary> A Guild-User pairing. </summary>
    public interface IGuildUser : IUser, IVoiceState, IComparable<IRole>
    {
        /// <summary> Gets when this user joined this guild. </summary>
        DateTimeOffset? JoinedAt { get; }
        /// <summary> Gets the nickname for this user. </summary>
        string Nickname { get; }
        GuildPermissions GuildPermissions { get; }

        /// <summary> Gets the guild for this user. </summary>
        IGuild Guild { get; }
        /// <summary> Gets the id of the guild for this user. </summary>
        ulong GuildId { get; }
        /// <summary> Returns a collection of the ids of the roles this user is a member of in this guild, including the guild's @everyone role. </summary>
        IReadOnlyCollection<ulong> RoleIds { get; }

        /// <summary> Gets the level permissions granted to this user to a given channel. </summary>
        ChannelPermissions GetPermissions(IGuildChannel channel);

        /// <summary> Kicks this user from this guild. </summary>
        Task KickAsync(RequestOptions options = null);
        /// <summary> Modifies this user's properties in this guild. </summary>
        Task ModifyAsync(Action<ModifyGuildMemberParams> func, RequestOptions options = null);

        /// <summary> The position of the user within the role hirearchy. </summary>
        /// <remarks> The returned value equal to the position of the highest role the user has, 
        /// or int.MaxValue if user is the server owner. </remarks>
        int Hirearchy { get; }
    }
}
