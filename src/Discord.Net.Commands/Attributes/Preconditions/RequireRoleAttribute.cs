using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public class RequireRoleAttribute : RequireGuildAttribute
    {
        public string Role { get; set; }
        public StringComparer Comparer { get; set; }

        public RequireRoleAttribute(string roleName)
        {
            Role = roleName;
            Comparer = StringComparer.Ordinal;
        }

        public RequireRoleAttribute(string roleName, StringComparer comparer)
        {
            Role = roleName;
            Comparer = comparer;
        }

        public override async Task<PreconditionResult> CheckPermissions(IMessage context)
        {
            var result = await base.CheckPermissions(context).ConfigureAwait(false);

            if (!result.IsSuccess)
                return result;

            var author = (context.Author as IGuildUser);

            if (author != null)
            {
                var hasRole = author.Roles.Any(x => Comparer.Compare(x.Name, Role) == 0);

                if (!hasRole)
                    return PreconditionResult.FromError($"User does not have the '{Role}' role.");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
