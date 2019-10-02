using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     An extension class for <see cref="IGuild"/>.
    /// </summary>
    public static class GuildExtensions
    {
        /// <summary>
        ///     Gets if welcome system messages are enabled.
        /// </summary>
        /// <param name="guild"> The guild to check. </param>
        /// <returns> A <c>bool</c> indicating if the welcome messages are enabled in the system channel. </returns>
        public static bool GetWelcomeMessagesEnabled(this IGuild guild)
            => !guild.SystemChannelFlags.HasFlag(SystemChannelMessageDeny.WelcomeMessage);

        /// <summary>
        ///     Gets if guild boost system messages are enabled.
        /// </summary>
        /// <param name="guild"> The guild to check. </param>
        /// <returns> A <c>bool</c> indicating if the guild boost messages are enabled in the system channel. </returns>
        public static bool GetGuildBoostMessagesEnabled(this IGuild guild)
            => !guild.SystemChannelFlags.HasFlag(SystemChannelMessageDeny.GuildBoost);

        /// <summary>
        ///     Creates a new role with the provided name.
        /// </summary>
        /// <param name="guild">The guild you want to create the role for.</param>
        /// <param name="func">The delegate to modify the properties of the role with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <c>null</c>.</exception>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the newly created
        ///     role.
        /// </returns>
        public static async Task<IRole> CreateRoleAsync(this IGuild guild, Action<RoleProperties> func, RequestOptions options = null)
        {
            if (func is null)
                throw new ArgumentNullException(nameof(func));

            var args = new RoleProperties();
            func(args);

            return await guild.CreateRoleAsync(
                       args.Name.GetValueOrDefault(),
                       args.Permissions.GetValueOrDefault(),
                       args.Color.GetValueOrDefault(),
                       args.Hoist.GetValueOrDefault(),
                       args.Mentionable.GetValueOrDefault(),
                       options);
        }
    }
}
