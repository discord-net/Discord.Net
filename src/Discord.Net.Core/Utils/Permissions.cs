using System.Linq;
using System.Runtime.CompilerServices;

namespace Discord
{
    internal static class Permissions
    {
        public const int MaxBits = 53;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PermValue GetValue(ulong allow, ulong deny, ChannelPermission flag)
            => GetValue(allow, deny, (ulong)flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PermValue GetValue(ulong allow, ulong deny, GuildPermission flag)
            => GetValue(allow, deny, (ulong)flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PermValue GetValue(ulong allow, ulong deny, ulong flag)
        {
            if (HasFlag(allow, flag))
                return PermValue.Allow;
            if (HasFlag(deny, flag))
                return PermValue.Deny;
            return PermValue.Inherit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(ulong value, ChannelPermission flag)
            => GetValue(value, (ulong)flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(ulong value, GuildPermission flag)
            => GetValue(value, (ulong)flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(ulong value, ulong flag) => HasFlag(value, flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong rawValue, bool? value, ChannelPermission flag)
            => SetValue(ref rawValue, value, (ulong)flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong rawValue, bool? value, GuildPermission flag)
            => SetValue(ref rawValue, value, (ulong)flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong rawValue, bool? value, ulong flag)
        {
            switch (value)
            {
                case null:
                    return;
                case true:
                    SetFlag(ref rawValue, flag);
                    break;
                default:
                    UnsetFlag(ref rawValue, flag);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong allow, ref ulong deny, PermValue? value, ChannelPermission flag)
            => SetValue(ref allow, ref deny, value, (ulong)flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong allow, ref ulong deny, PermValue? value, GuildPermission flag)
            => SetValue(ref allow, ref deny, value, (ulong)flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong allow, ref ulong deny, PermValue? value, ulong flag)
        {
            if (!value.HasValue) return;
            switch (value)
            {
                case PermValue.Allow:
                    SetFlag(ref allow, flag);
                    UnsetFlag(ref deny, flag);
                    break;
                case PermValue.Deny:
                    UnsetFlag(ref allow, flag);
                    SetFlag(ref deny, flag);
                    break;
                default:
                    UnsetFlag(ref allow, flag);
                    UnsetFlag(ref deny, flag);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasFlag(ulong value, ulong flag) => (value & flag) == flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFlag(ref ulong value, ulong flag) => value |= flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnsetFlag(ref ulong value, ulong flag) => value &= ~flag;

        public static ChannelPermissions ToChannelPerms(IGuildChannel channel, ulong guildPermissions)
            => new ChannelPermissions(guildPermissions & ChannelPermissions.All(channel).RawValue);

        public static ulong ResolveGuild(IGuild guild, IGuildUser user)
        {
            ulong resolvedPermissions = 0;

            if (user.Id == guild.OwnerId)
                resolvedPermissions = GuildPermissions.All.RawValue; //Owners always have all permissions
            else if (user.IsWebhook)
                resolvedPermissions = GuildPermissions.Webhook.RawValue;
            else
            {
                resolvedPermissions = user.RoleIds.Aggregate(resolvedPermissions, (current, roleId) => current | (guild.GetRole(roleId)?.Permissions.RawValue ?? 0));
                if (GetValue(resolvedPermissions, GuildPermission.Administrator))
                    resolvedPermissions = GuildPermissions.All.RawValue; //Administrators always have all permissions
            }

            return resolvedPermissions;
        }

        /*public static ulong ResolveChannel(IGuildUser user, IGuildChannel channel)
        {
            return ResolveChannel(user, channel, ResolveGuild(user));
        }*/
        public static ulong ResolveChannel(IGuild guild, IGuildUser user, IGuildChannel channel, ulong guildPermissions)
        {
            ulong resolvedPermissions;

            var mask = ChannelPermissions.All(channel).RawValue;
            if (GetValue(guildPermissions, GuildPermission.Administrator)) //Includes owner
                resolvedPermissions = mask; //Owners and administrators always have all permissions
            else
            {
                //Start with this user's guild permissions
                resolvedPermissions = guildPermissions;

                //Give/Take Everyone permissions
                var perms = channel.GetPermissionOverwrite(guild.EveryoneRole);
                if (perms != null)
                    resolvedPermissions = (resolvedPermissions & ~perms.Value.DenyValue) | perms.Value.AllowValue;

                //Give/Take Role permissions
                ulong deniedPermissions = 0UL, allowedPermissions = 0UL;
                foreach (var roleId in user.RoleIds)
                {
                    IRole role = null;
                    if (roleId == guild.EveryoneRole.Id || (role = guild.GetRole(roleId)) == null) continue;
                    perms = channel.GetPermissionOverwrite(role);
                    if (perms == null) continue;
                    allowedPermissions |= perms.Value.AllowValue;
                    deniedPermissions |= perms.Value.DenyValue;
                }

                resolvedPermissions = (resolvedPermissions & ~deniedPermissions) | allowedPermissions;

                //Give/Take User permissions
                perms = channel.GetPermissionOverwrite(user);
                if (perms != null)
                    resolvedPermissions = (resolvedPermissions & ~perms.Value.DenyValue) | perms.Value.AllowValue;

                switch (channel)
                {
                    case ITextChannel _ when !GetValue(resolvedPermissions, ChannelPermission.ViewChannel):
                        resolvedPermissions = 0;
                        break;
                    case ITextChannel _:
                        if (!GetValue(resolvedPermissions, ChannelPermission.SendMessages))
                        {
                            //No send permissions on a text channel removes all send-related permissions
                            resolvedPermissions &= ~(ulong)ChannelPermission.SendTTSMessages;
                            resolvedPermissions &= ~(ulong)ChannelPermission.MentionEveryone;
                            resolvedPermissions &= ~(ulong)ChannelPermission.EmbedLinks;
                            resolvedPermissions &= ~(ulong)ChannelPermission.AttachFiles;
                        }

                        break;
                }

                resolvedPermissions &=
                    mask; //Ensure we didnt get any permissions this channel doesnt support (from guildPerms, for example)
            }

            return resolvedPermissions;
        }
    }
}
