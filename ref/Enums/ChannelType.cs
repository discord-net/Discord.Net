using System;

namespace Discord
{
    [Flags]
    public enum ChannelType : byte
    {
        Public = 0x01,
        Private = 0x02,
        Text = 0x10,
        Voice = 0x20
    }
}
