using System;
using System.Globalization;

namespace Discord
{
    internal static class IdConvert
	{
		internal static readonly IFormatProvider _format = CultureInfo.InvariantCulture;
		
		public static ulong ToLong(string value)
			=> ulong.Parse(value, NumberStyles.None, _format);
		public static ulong? ToNullableLong(string value)
			=> value == null ? (ulong?)null : ulong.Parse(value, NumberStyles.None, _format);
		
		public static string ToString(ulong value)
			=> value.ToString(_format);
		public static string ToString(ulong? value)
			=> value?.ToString(_format);
	}
}
