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
    }
}
