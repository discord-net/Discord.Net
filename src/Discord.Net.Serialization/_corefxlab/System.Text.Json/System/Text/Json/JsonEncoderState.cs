// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Text.Json
{
    enum JsonEncoderState
    {
        UseFastUtf8 = 0,
        UseFastUtf16 = 1,
        UseFullEncoder = 2,
    }
}
