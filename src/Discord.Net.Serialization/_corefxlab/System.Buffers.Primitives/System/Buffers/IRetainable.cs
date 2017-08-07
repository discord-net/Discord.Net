// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Buffers
{
    public interface IRetainable
    {
        void Retain();
        void Release();
        bool IsRetained { get; }
    }
}
