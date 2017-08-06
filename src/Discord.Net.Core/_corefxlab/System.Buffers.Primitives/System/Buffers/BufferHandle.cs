// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Runtime.InteropServices;

namespace System.Buffers
{
    public unsafe struct BufferHandle : IDisposable
    {
        IRetainable _owner;
        void* _pointer;
        GCHandle _handle;

        public BufferHandle(IRetainable owner, void* pinnedPointer, GCHandle handle = default)
        {
            _pointer = pinnedPointer;
            _handle = handle;
            _owner = owner;
        }

        public BufferHandle(IRetainable owner) : this(owner, null) { }

        public void* PinnedPointer {
            get {
                if (_pointer == null) BufferPrimitivesThrowHelper.ThrowInvalidOperationException();
                return _pointer;
            }
        }

        public void Dispose()
        {
            if (_handle.IsAllocated) {
                _handle.Free();
            }

            if (_owner != null) {
                _owner.Release();
                _owner = null;
            }

            _pointer = null;
        }
    }
}
