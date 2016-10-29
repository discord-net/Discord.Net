using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    public static class GuildUserExtensions
    {
        public static Task AddRolesAsync(this IGuildUser user, params IRole[] roles)
            => AddRolesAsync(user, (IEnumerable<IRole>)roles);
        public static Task AddRolesAsync(this IGuildUser user, IEnumerable<IRole> roles)
            => user.ModifyAsync(x => x.RoleIds = user.RoleIds.Concat(roles.Select(y => y.Id)).ToArray());

        public static Task RemoveRolesAsync(this IGuildUser user, params IRole[] roles)
            => RemoveRolesAsync(user, (IEnumerable<IRole>)roles);
        public static Task RemoveRolesAsync(this IGuildUser user, IEnumerable<IRole> roles)
            => user.ModifyAsync(x => x.RoleIds = user.RoleIds.Except(roles.Select(y => y.Id)).ToArray());

        public static IEnumerable<IRole> GetRoles(this IGuildUser user) {
            var guild = user.Guild;
            return user.RoleIds.Select(r => guild.GetRole(r));
        }

        internal static int GetHirearchy(this IGuildUser user) {
            if(user == null)
                return -1;
            if(user.Id == user.Guild.OwnerId)
                return int.MaxValue;
            return user.GetRoles().Max(r => r.Position);
        }

        internal static int CompareRole(this IGuildUser user, IRole role) {
            if(user == null)
                return -1;
            if(role == null)
                return 1;
            return -user.Hirearchy.CompareTo(role.Position);
        }
    }
}
