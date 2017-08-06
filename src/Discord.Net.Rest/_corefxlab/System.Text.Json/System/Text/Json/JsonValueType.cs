// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Text.Json
{
    public enum JsonValueType
    {
        Unknown,
        Object,
        Array,
        Number,
        String,
        True,
        False,
        Null,
        Undefined,
        NaN,
        Infinity,
        NegativeInfinity,
    }
}
