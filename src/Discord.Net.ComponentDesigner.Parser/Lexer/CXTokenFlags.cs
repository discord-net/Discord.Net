using System;

namespace Discord.CX.Parser;

[Flags]
public enum CXTokenFlags : byte
{
    None = 0,
    Missing = 1 << 0
}
