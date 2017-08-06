// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

namespace System.Text.Utf16
{
    internal struct Utf16LittleEndianCodePointEnumerator : IEnumerator<uint>, IEnumerator
    {
        string _s;
        int _index;

        int _encodedChars;
        uint _codePoint;

        public Utf16LittleEndianCodePointEnumerator(string s)
        {
            _s = s;
            _index = -1;
            _encodedChars = 0;
            _codePoint = default;
        }

        public uint Current
        {
            get
            {
                if (_encodedChars != 0)
                {
                    return _codePoint;
                }

                if (_index < 0 || _index >= _s.Length)
                {
                    throw new InvalidOperationException("Enumerator is on invalid position");
                }

                if (!Utf8Helper.TryDecodeCodePointFromString(_s, _index, out _codePoint, out _encodedChars))
                {
                    _codePoint = default;
                    _encodedChars = 0;
                    // or index outside of string
                    throw new InvalidOperationException("Invalid characters in the string");
                }

                if (_encodedChars <= 0)
                {
                    // TODO: Change exception type
                    throw new Exception("Internal error: CodePoint is decoded but number of characters read is 0 or negative");
                }

                return _codePoint;
            }
        }

        public void Reset()
        {
            _index = -1;
            _encodedChars = 0;
            _codePoint = default;
        }

        public bool MoveNext()
        {
            if (_index == -1)
            {
                _index = 0;
                _encodedChars = 0;
            }
            else
            {
                uint dummy = Current;
                _index += _encodedChars;
                _encodedChars = 0;
            }

            return _index < _s.Length;
        }

        object IEnumerator.Current { get { return Current; } }

        void IDisposable.Dispose() { }
    }
}
