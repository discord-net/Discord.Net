// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Text.Json
{
    static class JsonConstants
    {
        #region Formatting constants

        public static readonly ParsedFormat NumberFormat = new ParsedFormat('D');
        public static readonly ParsedFormat DateTimeFormat = new ParsedFormat('O');
        public static readonly ParsedFormat GuidFormat = new ParsedFormat('D');

        #endregion Formatting constants

        #region Control characters

        public const byte OpenBrace = (byte)'{';
        public const byte CloseBrace = (byte)'}';
        public const byte OpenBracket = (byte)'[';
        public const byte CloseBracket = (byte)']';
        public const byte Space = (byte)' ';
        public const byte CarriageReturn = (byte)'\r';
        public const byte LineFeed = (byte)'\n';
        public const byte ListSeperator = (byte)',';
        public const byte KeyValueSeperator = (byte)':';
        public const byte Quote = (byte)'"';

        #endregion Control characters

        #region Common values

        public static readonly byte[] TrueValue = { (byte)'t', (byte)'r', (byte)'u', (byte)'e' };
        public static readonly byte[] FalseValue = { (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e' };
        public static readonly byte[] NullValue = { (byte)'n', (byte)'u', (byte)'l', (byte)'l' };

        #endregion Common values
    }
}
