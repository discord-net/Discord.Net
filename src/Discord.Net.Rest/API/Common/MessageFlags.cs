using System;

namespace Discord.API
{
    [Flags]
    internal enum MessageFlags : byte // probably safe to constrain this to 8 values, if not, it's internal so who cares
    {
        Suppressed = 0x04,
    }
}
