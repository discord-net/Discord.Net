// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Text.Json
{
    public enum JsonTokenType
    {
        None,
        StartObject,
        EndObject,
        StartArray,
        EndArray,
        PropertyName,
        Comment,
        Value,
        Null,
        Undefined,
    }
}
