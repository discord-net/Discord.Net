// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Buffers
{
    public interface IOutput
    {
        Span<byte> Buffer { get; }
        void Advance(int bytes);

        /// <summary>desiredBufferLength == 0 means "i don't care"</summary>
        void Enlarge(int desiredBufferLength = 0);
    }
}
