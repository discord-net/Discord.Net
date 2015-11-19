using System;
using System.Globalization;

namespace Discord
{
    internal static class IdConvert
	{
		internal static readonly IFormatProvider _format = CultureInfo.InvariantCulture;

		public static short ToShort(string value)
			=> short.Parse(value, NumberStyles.None, _format);
		public static short? ToNullableShort(string value)
			=> value == null ? (short?)null : short.Parse(value, NumberStyles.None, _format);
		public static long ToLong(string value)
			=> long.Parse(value, NumberStyles.None, _format);
		public static long? ToNullableLong(string value)
			=> value == null ? (long?)null : long.Parse(value, NumberStyles.None, _format);

		public static string ToString(short value)
			=> value.ToString(_format);
		public static string ToString(short? value)
			=> value?.ToString(_format);
		public static string ToString(long value)
			=> value.ToString(_format);
		public static string ToString(long? value)
			=> value?.ToString(_format);
	}
}
