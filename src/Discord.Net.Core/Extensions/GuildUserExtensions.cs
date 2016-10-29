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

        public static int CompareRoles(this IGuildUser left, IGuildUser right) {
            // These should never be empty since the everyone role is always present
            var roleLeft = left.GetRoles().Max();
            var roleRight= right.GetRoles().Max();
            return roleLeft.CompareTo(roleRight);
        }

        public static int Compare(this IGuildUser user, IRole role) {
            return user.GetRoles().Max().CompareTo(role);
        }
    }
}
