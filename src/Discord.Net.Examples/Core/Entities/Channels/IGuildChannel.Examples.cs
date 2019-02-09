using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Discord.Net.Examples.Core.Entities.Channels
{
    [PublicAPI]
    internal class GuildChannelExamples
    {
        #region AddPermissionOverwriteAsyncRole

        public async Task MuteRoleAsync(IRole role, IGuildChannel channel)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (channel == null) throw new ArgumentNullException(nameof(channel));

            // Fetches the previous overwrite and bail if one is found
            var previousOverwrite = channel.GetPermissionOverwrite(role);
            if (previousOverwrite.HasValue && previousOverwrite.Value.SendMessages == PermValue.Deny)
                throw new InvalidOperationException($"Role {role.Name} had already been muted in this channel.");

            // Creates a new OverwritePermissions with send message set to deny and pass it into the method
            await channel.AddPermissionOverwriteAsync(role, new OverwritePermissions(sendMessages: PermValue.Deny));
        }

        #endregion

        #region AddPermissionOverwriteAsyncUser

        public async Task MuteUserAsync(IGuildUser user, IGuildChannel channel)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (channel == null) throw new ArgumentNullException(nameof(channel));

            // Fetches the previous overwrite and bail if one is found
            var previousOverwrite = channel.GetPermissionOverwrite(user);
            if (previousOverwrite.HasValue && previousOverwrite.Value.SendMessages == PermValue.Deny)
                throw new InvalidOperationException($"User {user.Username} had already been muted in this channel.");

            // Creates a new OverwritePermissions with send message set to deny and pass it into the method
            await channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(sendMessages: PermValue.Deny));
        }

        #endregion
    }
}
