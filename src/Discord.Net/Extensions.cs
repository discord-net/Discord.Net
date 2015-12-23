using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Discord
{
    internal static class InternalExtensions
    {
		internal static readonly IFormatProvider _format = CultureInfo.InvariantCulture;
		
		public static ulong ToId(this string value)
			=> ulong.Parse(value, NumberStyles.None, _format);
		public static ulong? ToNullableId(this string value)
			=> value == null ? (ulong?)null : ulong.Parse(value, NumberStyles.None, _format);
		
		public static string ToIdString(this ulong value)
			=> value.ToString(_format);
		public static string ToIdString(this ulong? value)
			=> value?.ToString(_format);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasBit(this uint value, byte bit) => ((value >> bit) & 1U) == 1;

        public static bool TryGetAdd<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> d, 
            TKey key, Func<TKey, TValue> factory, out TValue result)
        {
            while (true)
            {
                if (d.TryGetValue(key, out result))
                    return false;
                if (d.TryAdd(key, factory(key)))
                    return true;
            }
        }
    }
}
