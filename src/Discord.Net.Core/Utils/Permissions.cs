using System.Runtime.CompilerServices;

namespace Discord
{
    internal static class Permissions
    {
        public const int MaxBits = 53;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PermValue GetValue(ulong allow, ulong deny, ChannelPermission bit)
            => GetValue(allow, deny, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PermValue GetValue(ulong allow, ulong deny, GuildPermission bit)
            => GetValue(allow, deny, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PermValue GetValue(ulong allow, ulong deny, byte bit)
        {
            if (HasBit(allow, bit))
                return PermValue.Allow;
            else if (HasBit(deny, bit))
                return PermValue.Deny;
            else
                return PermValue.Inherit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(ulong value, ChannelPermission bit)
            => GetValue(value, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(ulong value, GuildPermission bit)
            => GetValue(value, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(ulong value, byte bit) => HasBit(value, bit);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong rawValue, bool? value, ChannelPermission bit)
            => SetValue(ref rawValue, value, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong rawValue, bool? value, GuildPermission bit)
            => SetValue(ref rawValue, value, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong rawValue, bool? value, byte bit)
        {
            if (value.HasValue)
            {
                if (value == true)
                    SetBit(ref rawValue, bit);
                else
                    UnsetBit(ref rawValue, bit);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong allow, ref ulong deny, PermValue? value, ChannelPermission bit)
            => SetValue(ref allow, ref deny, value, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong allow, ref ulong deny, PermValue? value, GuildPermission bit)
            => SetValue(ref allow, ref deny, value, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong allow, ref ulong deny, PermValue? value, byte bit)
        {
            if (value.HasValue)
            {
                switch (value)
                {
                    case PermValue.Allow:
                        SetBit(ref allow, bit);
                        UnsetBit(ref deny, bit);
                        break;
                    case PermValue.Deny:
                        UnsetBit(ref allow, bit);
                        SetBit(ref deny, bit);
                        break;
                    default:
                        UnsetBit(ref allow, bit);
                        UnsetBit(ref deny, bit);
                        break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasBit(ulong value, byte bit) => (value & (1U << bit)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref ulong value, byte bit) => value |= (1U << bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnsetBit(ref ulong value, byte bit) => value &= ~(1U << bit);

        public static ulong ResolveGuild(IGuild guild, IGuildUser user)
        {
            ulong resolvedPermissions = 0;

            if (user.Id == guild.OwnerId)
                resolvedPermissions = GuildPermissions.All.RawValue; //Owners always have all permissions
            else
            {
                foreach (var roleId in user.RoleIds)
                    resolvedPermissions |= guild.GetRole(roleId)?.Permissions.RawValue ?? 0;
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
            ulong resolvedPermissions = 0;

            ulong mask = ChannelPermissions.All(channel).RawValue;
            if (/*user.Id == user.Guild.OwnerId || */GetValue(guildPermissions, GuildPermission.Administrator))
                resolvedPermissions = mask; //Owners and administrators always have all permissions
            else
            {
                //Start with this user's guild permissions
                resolvedPermissions = guildPermissions;

                OverwritePermissions? perms;
                var roleIds = user.RoleIds;
                if (roleIds.Count > 0)
                {
                    ulong deniedPermissions = 0UL, allowedPermissions = 0UL;
                    foreach (var roleId in roleIds)
                    {
                        perms = channel.GetPermissionOverwrite(guild.GetRole(roleId));
                        if (perms != null)
                        {
                            deniedPermissions |= perms.Value.DenyValue;
                            allowedPermissions |= perms.Value.AllowValue;
                        }
                    }
                    resolvedPermissions = (resolvedPermissions & ~deniedPermissions) | allowedPermissions;
                }
                perms = channel.GetPermissionOverwrite(user);
                if (perms != null)
                    resolvedPermissions = (resolvedPermissions & ~perms.Value.DenyValue) | perms.Value.AllowValue;

                //TODO: C#7 Typeswitch candidate
                var textChannel = channel as ITextChannel;
                var voiceChannel = channel as IVoiceChannel;
                if (textChannel != null && !GetValue(resolvedPermissions, ChannelPermission.ReadMessages))
                    resolvedPermissions = 0; //No read permission on a text channel removes all other permissions
                else if (voiceChannel != null && !GetValue(resolvedPermissions, ChannelPermission.Connect))
                    resolvedPermissions = 0; //No connect permission on a voice channel removes all other permissions
                resolvedPermissions &= mask; //Ensure we didnt get any permissions this channel doesnt support (from guildPerms, for example)
            }

            return resolvedPermissions;
        }
    }
}