// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Utf8;

namespace System.Text.Formatting
{
    public static class ITextOutputExtensions
    {
        public static void Append<TFormatter, T>(this TFormatter formatter, T value, ParsedFormat format = default) where T : IBufferFormattable where TFormatter : ITextOutput
        {
            while(!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter, T>(this TFormatter formatter, T value, ParsedFormat format = default) where T : IBufferFormattable where TFormatter : ITextOutput
        {
            int bytesWritten;
            if(!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, byte value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, byte value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            int bytesWritten;
            if(!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, sbyte value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, sbyte value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            int bytesWritten;
            if (!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, ushort value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, ushort value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            int bytesWritten;
            if (!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, short value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, short value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            int bytesWritten;
            if (!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, uint value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, uint value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            int bytesWritten;
            if (!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, int value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, int value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            int bytesWritten;
            if (!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, ulong value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, ulong value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            int bytesWritten;
            if (!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, long value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, long value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            int bytesWritten;
            if (!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, char value) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, char value) where TFormatter : ITextOutput
        {
            return formatter.TryAppend(value, formatter.SymbolTable);
        }

        public static void Append<TFormatter>(this TFormatter formatter, ReadOnlySpan<char> value) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, ReadOnlySpan<char> value) where TFormatter : ITextOutput
        {
            return formatter.TryAppend(value, formatter.SymbolTable);
        }

        public static void Append<TFormatter>(this TFormatter formatter, string value) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, string value) where TFormatter : ITextOutput
        {
            return formatter.TryAppend(value, formatter.SymbolTable);
        }

        public static void Append<TFormatter>(this TFormatter formatter, Utf8String value) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, Utf8String value) where TFormatter : ITextOutput
        {
            int bytesWritten;
            int consumed;
            if (!formatter.SymbolTable.TryEncode(value, formatter.Buffer, out consumed, out bytesWritten)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, Guid value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, Guid value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            int bytesWritten;
            if (!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, DateTime value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, DateTime value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            int bytesWritten;
            if (!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, DateTimeOffset value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, DateTimeOffset value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            int bytesWritten;
            if (!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, TimeSpan value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, TimeSpan value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            int bytesWritten;
            if (!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, float value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, float value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            int bytesWritten;
            if (!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }

        public static void Append<TFormatter>(this TFormatter formatter, double value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            while (!formatter.TryAppend(value, format)) {
                formatter.Enlarge();
            }
        }

        public static bool TryAppend<TFormatter>(this TFormatter formatter, double value, ParsedFormat format = default) where TFormatter : ITextOutput
        {
            int bytesWritten;
            if (!value.TryFormat(formatter.Buffer, out bytesWritten, format, formatter.SymbolTable)) {
                return false;
            }
            formatter.Advance(bytesWritten);
            return true;
        }
    }
}
