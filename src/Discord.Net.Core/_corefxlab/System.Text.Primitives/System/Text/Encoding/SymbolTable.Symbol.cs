// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Text
{
    public partial class SymbolTable
    {
        // Do not change the specific enum values without careful consideration of the impacts to the parsers.
        public enum Symbol : ushort {
            D0 = (ushort)0,
            D1 = (ushort)1,
            D2 = (ushort)2,
            D3 = (ushort)3,
            D4 = (ushort)4,
            D5 = (ushort)5,
            D6 = (ushort)6,
            D7 = (ushort)7,
            D8 = (ushort)8,
            D9 = (ushort)9,
            DecimalSeparator = (ushort)10,
            Exponent = (ushort)16,
            GroupSeparator = (ushort)11,
            InfinitySign = (ushort)12,
            MinusSign = (ushort)13,
            NaN = (ushort)15,
            PlusSign = (ushort)14,
        }
    }
}
