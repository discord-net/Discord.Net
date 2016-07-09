using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.API.Rest;

namespace Discord
{
    public interface IRole : IDeletable, ISnowflakeEntity
    {
        /// <summary> Gets the color given to users of this role. </summary>
        Color Color { get; }
        /// <summary> Returns true if users of this role are separated in the user list. </summary>
        bool IsHoisted { get; }
        /// <summary> Returns true if this role is automatically managed by Discord. </summary>
        bool IsManaged { get; }
        /// <summary> Gets the name of this role. </summary>
        string Name { get; }
        /// <summary> Gets the permissions granted to members of this role. </summary>
        GuildPermissions Permissions { get; }
        /// <summary> Gets this role's position relative to other roles in the same guild. </summary>
        int Position { get; }

        /// <summary> Gets the id of the guild owning this role.</summary>
        ulong GuildId { get; }

        /// <summary> Modifies this role. </summary>
        Task ModifyAsync(Action<ModifyGuildRoleParams> func);
    }
}