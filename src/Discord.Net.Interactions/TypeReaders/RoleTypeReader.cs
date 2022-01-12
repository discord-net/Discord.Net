using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     A <see cref="TypeReader"/> for parsing objects implementing <see cref="IRole"/>.
    /// </summary>
    /// <typeparam name="T">The type to be checked; must implement <see cref="IRole"/>.</typeparam>
    internal class RoleTypeReader<T> : TypeReader<T> where T : class, IRole
    {
        /// <inheritdoc />
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, object input, IServiceProvider services)
        {
            if (context.Guild is not null)
            {
                if (ulong.TryParse(input as string, out var id))
                {
                    var role = context.Guild.GetRole(id);

                    if (role is not null)
                        return Task.FromResult(TypeConverterResult.FromSuccess(role as T));
                }

                if (MentionUtils.TryParseRole(input as string, out id))
                {
                    var role = context.Guild.GetRole(id);

                    if (role is not null)
                        return Task.FromResult(TypeConverterResult.FromSuccess(role as T));
                }

                var channels = context.Guild.Roles;
                var nameMatch = channels.First(x => string.Equals(x, input as string));

                if (nameMatch is not null)
                    return Task.FromResult(TypeConverterResult.FromSuccess(nameMatch as T));
            }

            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "Role not found."));
        }
    }
}
