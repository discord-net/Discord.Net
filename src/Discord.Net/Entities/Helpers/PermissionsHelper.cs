using System.Runtime.CompilerServices;

namespace Discord
{
    internal static class PermissionsHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PermValue GetValue(uint allow, uint deny, PermissionBit bit)
        {
            if (HasBit(ref allow, (byte)bit))
                return PermValue.Allow;
            else if (HasBit(ref deny, (byte)bit))
                return PermValue.Deny;
            else
                return PermValue.Inherit;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(uint value, PermissionBit bit) => HasBit(ref value, (byte)bit);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref uint rawValue, bool? value, PermissionBit bit)
        {
            if (value.HasValue)
            {
                if (value == true)
                    SetBit(ref rawValue, (byte)bit);
                else
                    UnsetBit(ref rawValue, (byte)bit);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref uint allow, ref uint deny, PermValue? value, PermissionBit bit)
        {
            if (value.HasValue)
            {
                switch (value)
                {
                    case PermValue.Allow:
                        SetBit(ref allow, (byte)bit);
                        UnsetBit(ref deny, (byte)bit);
                        break;
                    case PermValue.Deny:
                        UnsetBit(ref allow, (byte)bit);
                        SetBit(ref deny, (byte)bit);
                        break;
                    default:
                        UnsetBit(ref allow, (byte)bit);
                        UnsetBit(ref deny, (byte)bit);
                        break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasBit(ref uint value, byte bit) => (value & (1U << bit)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref uint value, byte bit) => value |= (1U << bit);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnsetBit(ref uint value, byte bit) => value &= ~(1U << bit);
    }
}
