using System.Runtime.CompilerServices;

namespace Discord
{
    internal static class PermissionUtilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PermValue GetValue(uint allow, uint deny, ChannelPermission bit)
            => GetValue(allow, deny, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PermValue GetValue(uint allow, uint deny, GuildPermission bit)
            => GetValue(allow, deny, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PermValue GetValue(uint allow, uint deny, byte bit)
        {
            if (HasBit(allow, bit))
                return PermValue.Allow;
            else if (HasBit(deny, bit))
                return PermValue.Deny;
            else
                return PermValue.Inherit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(uint value, ChannelPermission bit)
            => GetValue(value, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(uint value, GuildPermission bit)
            => GetValue(value, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(uint value, byte bit) => HasBit(value, bit);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref uint rawValue, bool? value, ChannelPermission bit)
            => SetValue(ref rawValue, value, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref uint rawValue, bool? value, GuildPermission bit)
            => SetValue(ref rawValue, value, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref uint rawValue, bool? value, byte bit)
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
        public static void SetValue(ref uint allow, ref uint deny, PermValue? value, ChannelPermission bit)
            => SetValue(ref allow, ref deny, value, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref uint allow, ref uint deny, PermValue? value, GuildPermission bit)
            => SetValue(ref allow, ref deny, value, (byte)bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref uint allow, ref uint deny, PermValue? value, byte bit)
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
        private static bool HasBit(uint value, byte bit) => (value & (1U << bit)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref uint value, byte bit) => value |= (1U << bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnsetBit(ref uint value, byte bit) => value &= ~(1U << bit);
    }
}
