namespace Discord.Rest
{
    public static class PermissionHelper
    {
        public static uint Resolve(IGuildUser user)
        {
            var roles = user.Roles;
            uint newPermissions = 0;
            for (int i = 0; i < roles.Count; i++)
                newPermissions |= roles[i].Permissions.RawValue;
            return newPermissions;
        }

        public static uint Resolve(IGuildUser user, IGuildChannel channel)
        {
            uint resolvedPermissions = 0;

            uint mask = ChannelPermissions.All(channel).RawValue;
            if (user.Id == user.Guild.OwnerId || PermissionUtilities.GetValue(resolvedPermissions, GuildPermission.Administrator))
                resolvedPermissions = mask; //Owners and administrators always have all permissions
            else
            {
                //Start with this user's guild permissions
                resolvedPermissions = Resolve(user);
                var overwrites = channel.PermissionOverwrites;

                Overwrite entry;
                var roles = user.Roles;
                if (roles.Count > 0)
                {
                    for (int i = 0; i < roles.Count; i++)
                    {
                        if (overwrites.TryGetValue(roles[i].Id, out entry))
                            resolvedPermissions &= ~entry.Permissions.DenyValue;
                    }
                    for (int i = 0; i < roles.Count; i++)
                    {
                        if (overwrites.TryGetValue(roles[i].Id, out entry))
                            resolvedPermissions |= entry.Permissions.AllowValue;
                    }
                }
                if (overwrites.TryGetValue(user.Id, out entry))
                    resolvedPermissions = (resolvedPermissions & ~entry.Permissions.DenyValue) | entry.Permissions.AllowValue;

                switch (channel)
                {
                    case TextChannel _:
                        if (!PermissionUtilities.GetValue(resolvedPermissions, ChannelPermission.ReadMessages))
                            resolvedPermissions = 0; //No read permission on a text channel removes all other permissions
                        break;
                    case VoiceChannel _:
                        if (!PermissionUtilities.GetValue(resolvedPermissions, ChannelPermission.Connect))
                            resolvedPermissions = 0; //No read permission on a text channel removes all other permissions
                        break;
                    default:
                        resolvedPermissions &= mask; //Ensure we didnt get any permissions this channel doesnt support (from guildPerms, for example)
                        break;
                }
            }

            return resolvedPermissions;
        }
    }
}
