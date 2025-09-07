using System;

namespace Discord.ComponentDesignerGenerator.Parser;

[Flags]
public enum CXTokenFlags : byte
{
    None = 0,
    HasErrors = 1 << 0
}
