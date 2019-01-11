using System;

namespace Discord
{
    [Flags]
    public enum ActivityFlag
    {
        Instance = 1,
        Join = 0b10,
        Spectate = 0b100,
        JoinRequest = 0b1000,
        Sync = 0b10000,
        Play = 0b100000
    }
}
