using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.API.Rest;

namespace Discord
{
    /// <summary> A Guild-User pairing. </summary>
    public interface IGuildUser : IUpdateable, IUser
    {
        /// <summary> Returns true if the guild has deafened this user. </summary>
        bool IsDeaf { get; }
        /// <summary> Returns true if the guild has muted this user. </summary>
        bool IsMute { get; }
        /// <summary> Gets when this user joined this guild. </summary>
        DateTime JoinedAt { get; }
        /// <summary> Gets the nickname for this user. </summary>
        string Nickname { get; }
        /// <summary> Gets the guild-level permissions granted to this user by their roles. </summary>
        GuildPermissions GuildPermissions { get; }

        /// <summary> Gets the guild for this guild-user pair. </summary>
        IGuild Guild { get; }
        /// <summary> Returns a collection of the roles this user is a member of in this guild, including the guild's @everyone role. </summary>
        IReadOnlyCollection<IRole> Roles { get; }
        /// <summary> Gets the voice channel this user is currently in, if any. </summary>
        IVoiceChannel VoiceChannel { get; }

        /// <summary> Gets the channel-level permissions granted to this user for a given channel. </summary>
        ChannelPermissions GetPermissions(IGuildChannel channel);

        /// <summary> Kicks this user from this guild. </summary>
        Task Kick();
        /// <summary> Modifies this user's properties in this guild. </summary>
        Task Modify(Action<ModifyGuildMemberParams> func);
    }
}
