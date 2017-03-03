using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    public static class GuildUserExtensions
    {
        public static Task AddRolesAsync(this IGuildUser user, params IRole[] roles)
            => ChangeRolesAsync(user, add: roles);
        public static Task AddRolesAsync(this IGuildUser user, IEnumerable<IRole> roles)
            => ChangeRolesAsync(user, add: roles);
        public static Task RemoveRolesAsync(this IGuildUser user, params IRole[] roles)
            => ChangeRolesAsync(user, remove: roles);
        public static Task RemoveRolesAsync(this IGuildUser user, IEnumerable<IRole> roles)
            => ChangeRolesAsync(user, remove: roles);
        public static async Task ChangeRolesAsync(this IGuildUser user, IEnumerable<IRole> add = null, IEnumerable<IRole> remove = null)
        {
            IEnumerable<ulong> roleIds = user.RoleIds;
            if (remove != null)
                roleIds = roleIds.Except(remove.Select(x => x.Id));
            if (add != null)
                roleIds = roleIds.Concat(add.Select(x => x.Id));
            await user.ModifyAsync(x => x.RoleIds = roleIds.ToArray()).ConfigureAwait(false);
        }
    }
}
