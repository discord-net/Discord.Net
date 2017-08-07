// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace System.Text.Utf8
{
    partial struct Utf8String
    {
        public struct CodePointEnumerable 
        {
            private ReadOnlySpan<byte> _buffer;

            public CodePointEnumerable(byte[] bytes, int index, int length)
            {
                _buffer = new ReadOnlySpan<byte>(bytes, index, length);
            }

            public unsafe CodePointEnumerable(ReadOnlySpan<byte> buffer)
            {
                _buffer = buffer;
            }

            public CodePointEnumerator GetEnumerator()
            {
                return new CodePointEnumerator(_buffer);
            }

            public CodePointReverseEnumerator GetReverseEnumerator()
            {
                return new CodePointReverseEnumerator(_buffer);
            }

            public int Count()
            {
                int result = 0;
                foreach (var cp in this)
                {
                    result++;
                }

                return result;
            }
        }
    }
}
