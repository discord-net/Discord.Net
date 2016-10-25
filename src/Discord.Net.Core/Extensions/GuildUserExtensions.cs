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

        internal static int Compare(this IGuildUser u1, IGuildUser u2) {
            // These should never be empty since the everyone role is always present
            var r1 = u1.GetRoles().Max();
            var r2 = u2.GetRoles().Max();
            var result = r1.CompareTo(r2);
            return result != 0 ? result : u1.Id.CompareTo(u2.Id);
        }

        internal static int Compare(this IGuildUser user, IRole role) {
            return user.GetRoles().Max().CompareTo(role);
        }
    }
}
