// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

namespace System.Text.Formatting
{
    public struct OutputFormatter<TOutput> : ITextOutput where TOutput : IOutput
    {
        TOutput _output;
        SymbolTable _symbolTable;

        public OutputFormatter(TOutput output, SymbolTable symbolTable)
        {
            _output = output;
            _symbolTable = symbolTable;
        }

        public OutputFormatter(TOutput output) : this(output, SymbolTable.InvariantUtf8)
        {
        }

        public Span<byte> Buffer => _output.Buffer;

        public SymbolTable SymbolTable => _symbolTable;

        public void Advance(int bytes) => _output.Advance(bytes);

        public void Enlarge(int desiredBufferLength = 0) => _output.Enlarge(desiredBufferLength);
    }
}
