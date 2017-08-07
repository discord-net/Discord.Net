// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace System.Text
{
    internal static class InvariantUtf8TimeFormatter
    {
        #region Constants

        private const int DefaultFractionDigits = 7;

        private const byte Colon = (byte)':';
        private const byte Comma = (byte)',';
        private const byte Minus = (byte)'-';
        private const byte Period = (byte)'.';
        private const byte Plus = (byte)'+';
        private const byte Slash = (byte)'/';
        private const byte Space = (byte)' ';

        private const byte TimeMarker = (byte)'T';
        private const byte UtcMarker = (byte)'Z';

        private const byte GMT1 = (byte)'G';
        private const byte GMT2 = (byte)'M';
        private const byte GMT3 = (byte)'T';

        private const byte GMT1Lowercase = (byte)'g';
        private const byte GMT2Lowercase = (byte)'m';
        private const byte GMT3Lowercase = (byte)'t';

        private static readonly byte[][] DayAbbreviations = new byte[][]
        {
                new byte[] { (byte)'S', (byte)'u', (byte)'n' },
                new byte[] { (byte)'M', (byte)'o', (byte)'n' },
                new byte[] { (byte)'T', (byte)'u', (byte)'e' },
                new byte[] { (byte)'W', (byte)'e', (byte)'d' },
                new byte[] { (byte)'T', (byte)'h', (byte)'u' },
                new byte[] { (byte)'F', (byte)'r', (byte)'i' },
                new byte[] { (byte)'S', (byte)'a', (byte)'t' },
        };

        private static readonly byte[][] DayAbbreviationsLowercase = new byte[][]
        {
                new byte[] { (byte)'s', (byte)'u', (byte)'n' },
                new byte[] { (byte)'m', (byte)'o', (byte)'n' },
                new byte[] { (byte)'t', (byte)'u', (byte)'e' },
                new byte[] { (byte)'w', (byte)'e', (byte)'d' },
                new byte[] { (byte)'t', (byte)'h', (byte)'u' },
                new byte[] { (byte)'f', (byte)'r', (byte)'i' },
                new byte[] { (byte)'s', (byte)'a', (byte)'t' },
        };

        private static readonly byte[][] MonthAbbreviations = new byte[][]
        {
                new byte[] { (byte)'J', (byte)'a', (byte)'n' },
                new byte[] { (byte)'F', (byte)'e', (byte)'b' },
                new byte[] { (byte)'M', (byte)'a', (byte)'r' },
                new byte[] { (byte)'A', (byte)'p', (byte)'r' },
                new byte[] { (byte)'M', (byte)'a', (byte)'y' },
                new byte[] { (byte)'J', (byte)'u', (byte)'n' },
                new byte[] { (byte)'J', (byte)'u', (byte)'l' },
                new byte[] { (byte)'A', (byte)'u', (byte)'g' },
                new byte[] { (byte)'S', (byte)'e', (byte)'p' },
                new byte[] { (byte)'O', (byte)'c', (byte)'t' },
                new byte[] { (byte)'N', (byte)'o', (byte)'v' },
                new byte[] { (byte)'D', (byte)'e', (byte)'c' },
        };

        private static readonly byte[][] MonthAbbreviationsLowercase = new byte[][]
{
                new byte[] { (byte)'j', (byte)'a', (byte)'n' },
                new byte[] { (byte)'f', (byte)'e', (byte)'b' },
                new byte[] { (byte)'m', (byte)'a', (byte)'r' },
                new byte[] { (byte)'a', (byte)'p', (byte)'r' },
                new byte[] { (byte)'m', (byte)'a', (byte)'y' },
                new byte[] { (byte)'j', (byte)'u', (byte)'n' },
                new byte[] { (byte)'j', (byte)'u', (byte)'l' },
                new byte[] { (byte)'a', (byte)'u', (byte)'g' },
                new byte[] { (byte)'s', (byte)'e', (byte)'p' },
                new byte[] { (byte)'o', (byte)'c', (byte)'t' },
                new byte[] { (byte)'n', (byte)'o', (byte)'v' },
                new byte[] { (byte)'d', (byte)'e', (byte)'c' },
};

        #endregion Constants

        public static bool TryFormatG(DateTime value, TimeSpan offset, Span<byte> buffer, out int bytesWritten)
        {
            const int MinimumBytesNeeded = 19;

            bytesWritten = MinimumBytesNeeded;
            if (offset != PrimitiveFormatter.NullOffset)
            {
                bytesWritten += 7; // Space['+'|'-']hh:ss
            }

            if (buffer.Length < bytesWritten)
            {
                bytesWritten = 0;
                return false;
            }

            ref byte utf8Bytes = ref buffer.DangerousGetPinnableReference();

            FormattingHelpers.WriteDigits(value.Month, 2, ref utf8Bytes, 0);
            Unsafe.Add(ref utf8Bytes, 2) = Slash;

            FormattingHelpers.WriteDigits(value.Day, 2, ref utf8Bytes, 3);
            Unsafe.Add(ref utf8Bytes, 5) = Slash;

            FormattingHelpers.WriteDigits(value.Year, 4, ref utf8Bytes, 6);
            Unsafe.Add(ref utf8Bytes, 10) = Space;

            FormattingHelpers.WriteDigits(value.Hour, 2, ref utf8Bytes, 11);
            Unsafe.Add(ref utf8Bytes, 13) = Colon;

            FormattingHelpers.WriteDigits(value.Minute, 2, ref utf8Bytes, 14);
            Unsafe.Add(ref utf8Bytes, 16) = Colon;

            FormattingHelpers.WriteDigits(value.Second, 2, ref utf8Bytes, 17);

            if (offset != PrimitiveFormatter.NullOffset)
            {
                Unsafe.Add(ref utf8Bytes, 19) = Space;

                long ticks = value.Ticks;
                if (ticks < 0)
                {
                    Unsafe.Add(ref utf8Bytes, 20) = Minus;
                    ticks = -ticks;
                }
                else
                {
                    Unsafe.Add(ref utf8Bytes, 20) = Plus;
                }

                FormattingHelpers.WriteDigits(offset.Hours, 2, ref utf8Bytes, 21);
                Unsafe.Add(ref utf8Bytes, 23) = Colon;
                FormattingHelpers.WriteDigits(offset.Minutes, 2, ref utf8Bytes, 24);
            }

            return true;
        }

        public static bool TryFormatO(DateTime value, TimeSpan offset, Span<byte> buffer, out int bytesWritten)
        {
            const int MinimumBytesNeeded = 27;

            bytesWritten = MinimumBytesNeeded;
            DateTimeKind kind = DateTimeKind.Local;

            if (offset == PrimitiveFormatter.NullOffset)
            {
                kind = value.Kind;
                if (kind == DateTimeKind.Local)
                {
                    offset = TimeZoneInfo.Local.GetUtcOffset(value);
                    bytesWritten += 6;
                }
                else if (kind == DateTimeKind.Utc)
                {
                    bytesWritten += 1;
                }
            }
            else
            {
                bytesWritten += 6;
            }

            if (buffer.Length < bytesWritten)
            {
                bytesWritten = 0;
                return false;
            }

            ref byte utf8Bytes = ref buffer.DangerousGetPinnableReference();

            FormattingHelpers.WriteDigits(value.Year, 4, ref utf8Bytes, 0);
            Unsafe.Add(ref utf8Bytes, 4) = Minus;

            FormattingHelpers.WriteDigits(value.Month, 2, ref utf8Bytes, 5);
            Unsafe.Add(ref utf8Bytes, 7) = Minus;

            FormattingHelpers.WriteDigits(value.Day, 2, ref utf8Bytes, 8);
            Unsafe.Add(ref utf8Bytes, 10) = TimeMarker;

            FormattingHelpers.WriteDigits(value.Hour, 2, ref utf8Bytes, 11);
            Unsafe.Add(ref utf8Bytes, 13) = Colon;

            FormattingHelpers.WriteDigits(value.Minute, 2, ref utf8Bytes, 14);
            Unsafe.Add(ref utf8Bytes, 16) = Colon;

            FormattingHelpers.WriteDigits(value.Second, 2, ref utf8Bytes, 17);
            Unsafe.Add(ref utf8Bytes, 19) = Period;

            FormattingHelpers.DivMod(value.Ticks, TimeSpan.TicksPerSecond, out long fraction);
            FormattingHelpers.WriteDigits(fraction, DefaultFractionDigits, ref utf8Bytes, 20);

            if (kind == DateTimeKind.Local)
            {
                int hours = offset.Hours;
                byte sign = Plus;

                if (offset.Hours < 0)
                {
                    hours = -offset.Hours;
                    sign = Minus;
                }

                Unsafe.Add(ref utf8Bytes, 27) = sign;
                FormattingHelpers.WriteDigits(hours, 2, ref utf8Bytes, 28);
                Unsafe.Add(ref utf8Bytes, 30) = Colon;
                FormattingHelpers.WriteDigits(offset.Minutes, 2, ref utf8Bytes, 31);
            }
            else if (kind == DateTimeKind.Utc)
            {
                Unsafe.Add(ref utf8Bytes, 27) = UtcMarker;
            }

            return true;
        }

        public static bool TryFormatRfc1123(DateTime value, Span<byte> buffer, out int bytesWritten)
        {
            const int BytesNeeded = 29;

            bytesWritten = BytesNeeded;
            if (buffer.Length < bytesWritten)
            {
                bytesWritten = 0;
                return false;
            }

            ref byte utf8Bytes = ref buffer.DangerousGetPinnableReference();

            var dayAbbrev = DayAbbreviations[(int)value.DayOfWeek];
            Unsafe.Add(ref utf8Bytes, 0) = dayAbbrev[0];
            Unsafe.Add(ref utf8Bytes, 1) = dayAbbrev[1];
            Unsafe.Add(ref utf8Bytes, 2) = dayAbbrev[2];
            Unsafe.Add(ref utf8Bytes, 3) = Comma;
            Unsafe.Add(ref utf8Bytes, 4) = Space;

            FormattingHelpers.WriteDigits(value.Day, 2, ref utf8Bytes, 5);
            Unsafe.Add(ref utf8Bytes, 7) = (byte)' ';

            var monthAbbrev = MonthAbbreviations[value.Month - 1];
            Unsafe.Add(ref utf8Bytes, 8) = monthAbbrev[0];
            Unsafe.Add(ref utf8Bytes, 9) = monthAbbrev[1];
            Unsafe.Add(ref utf8Bytes, 10) = monthAbbrev[2];
            Unsafe.Add(ref utf8Bytes, 11) = Space;

            FormattingHelpers.WriteDigits(value.Year, 4, ref utf8Bytes, 12);
            Unsafe.Add(ref utf8Bytes, 16) = Space;

            FormattingHelpers.WriteDigits(value.Hour, 2, ref utf8Bytes, 17);
            Unsafe.Add(ref utf8Bytes, 19) = Colon;

            FormattingHelpers.WriteDigits(value.Minute, 2, ref utf8Bytes, 20);
            Unsafe.Add(ref utf8Bytes, 22) = Colon;

            FormattingHelpers.WriteDigits(value.Second, 2, ref utf8Bytes, 23);
            Unsafe.Add(ref utf8Bytes, 25) = Space;

            Unsafe.Add(ref utf8Bytes, 26) = GMT1;
            Unsafe.Add(ref utf8Bytes, 27) = GMT2;
            Unsafe.Add(ref utf8Bytes, 28) = GMT3;

            return true;
        }

        public static bool TryFormatRfc1123Lowercase(DateTime value, Span<byte> buffer, out int bytesWritten)
        {
            const int BytesNeeded = 29;

            bytesWritten = BytesNeeded;
            if (buffer.Length < bytesWritten)
            {
                bytesWritten = 0;
                return false;
            }

            ref byte utf8Bytes = ref buffer.DangerousGetPinnableReference();

            var dayAbbrev = DayAbbreviationsLowercase[(int)value.DayOfWeek];
            Unsafe.Add(ref utf8Bytes, 0) = dayAbbrev[0];
            Unsafe.Add(ref utf8Bytes, 1) = dayAbbrev[1];
            Unsafe.Add(ref utf8Bytes, 2) = dayAbbrev[2];
            Unsafe.Add(ref utf8Bytes, 3) = Comma;
            Unsafe.Add(ref utf8Bytes, 4) = Space;

            FormattingHelpers.WriteDigits(value.Day, 2, ref utf8Bytes, 5);
            Unsafe.Add(ref utf8Bytes, 7) = (byte)' ';

            var monthAbbrev = MonthAbbreviationsLowercase[value.Month - 1];
            Unsafe.Add(ref utf8Bytes, 8) = monthAbbrev[0];
            Unsafe.Add(ref utf8Bytes, 9) = monthAbbrev[1];
            Unsafe.Add(ref utf8Bytes, 10) = monthAbbrev[2];
            Unsafe.Add(ref utf8Bytes, 11) = Space;

            FormattingHelpers.WriteDigits(value.Year, 4, ref utf8Bytes, 12);
            Unsafe.Add(ref utf8Bytes, 16) = Space;

            FormattingHelpers.WriteDigits(value.Hour, 2, ref utf8Bytes, 17);
            Unsafe.Add(ref utf8Bytes, 19) = Colon;

            FormattingHelpers.WriteDigits(value.Minute, 2, ref utf8Bytes, 20);
            Unsafe.Add(ref utf8Bytes, 22) = Colon;

            FormattingHelpers.WriteDigits(value.Second, 2, ref utf8Bytes, 23);
            Unsafe.Add(ref utf8Bytes, 25) = Space;

            Unsafe.Add(ref utf8Bytes, 26) = GMT1Lowercase;
            Unsafe.Add(ref utf8Bytes, 27) = GMT2Lowercase;
            Unsafe.Add(ref utf8Bytes, 28) = GMT3Lowercase;

            return true;
        }

        public static bool TryFormat(TimeSpan value, char format, Span<byte> buffer, out int bytesWritten)
        {
            bool longForm = (format == 'G');
            bool constant = (format == 't' || format == 'T' || format == 'c');

            long ticks = value.Ticks;
            int days = (int)FormattingHelpers.DivMod(ticks, TimeSpan.TicksPerDay, out long timeLeft);

            bool showSign = false;
            if (ticks < 0)
            {
                showSign = true;
                days = -days;
                timeLeft = -timeLeft;
            }

            int hours = (int)FormattingHelpers.DivMod(timeLeft, TimeSpan.TicksPerHour, out timeLeft);
            int minutes = (int)FormattingHelpers.DivMod(timeLeft, TimeSpan.TicksPerMinute, out timeLeft);
            int seconds = (int)FormattingHelpers.DivMod(timeLeft, TimeSpan.TicksPerSecond, out long fraction);

            int dayDigits = 0;
            int hourDigits = (constant || longForm || hours > 9) ? 2 : 1;
            int fractionDigits = 0;

            bytesWritten = hourDigits + 6; // [h]h:mm:ss
            if (showSign)
                bytesWritten += 1;  // [-]
            if (longForm || days > 0)
            {
                dayDigits = FormattingHelpers.CountDigits(days);
                bytesWritten += dayDigits + 1; // [d'.']
            }
            if (longForm || fraction > 0)
            {
                fractionDigits = (longForm || constant) ? DefaultFractionDigits : FormattingHelpers.CountFractionDigits(fraction);
                bytesWritten += fractionDigits + 1; // ['.'fffffff] or ['.'FFFFFFF] for short-form
            }

            if (buffer.Length < bytesWritten)
            {
                bytesWritten = 0;
                return false;
            }

            ref byte utf8Bytes = ref buffer.DangerousGetPinnableReference();
            int idx = 0;

            if (showSign)
                Unsafe.Add(ref utf8Bytes, idx++) = Minus;

            if (dayDigits > 0)
            {
                idx += FormattingHelpers.WriteDigits(days, dayDigits, ref utf8Bytes, idx);
                Unsafe.Add(ref utf8Bytes, idx++) = constant ? Period : Colon;
            }

            idx += FormattingHelpers.WriteDigits(hours, hourDigits, ref utf8Bytes, idx);
            Unsafe.Add(ref utf8Bytes, idx++) = Colon;

            idx += FormattingHelpers.WriteDigits(minutes, 2, ref utf8Bytes, idx);
            Unsafe.Add(ref utf8Bytes, idx++) = Colon;

            idx += FormattingHelpers.WriteDigits(seconds, 2, ref utf8Bytes, idx);

            if (fractionDigits > 0)
            {
                Unsafe.Add(ref utf8Bytes, idx++) = Period;
                idx += FormattingHelpers.WriteFractionDigits(fraction, fractionDigits, ref utf8Bytes, idx);
            }

            return true;
        }
    }
}
