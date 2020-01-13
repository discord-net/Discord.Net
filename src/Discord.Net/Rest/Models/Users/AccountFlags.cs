using System;

namespace Discord.Models
{
    [Flags]
    public enum AccountFlags : short
    {
        None = 0,
        Employee = 1<<0,
        Partner = 1<<1,
        HypesquadEvents = 1<<2,
        BugHunter = 1<<3,
        HypesquadBravery = 1<<6,
        HypesquadBrilliance = 1<<7,
        HypesquadBalance = 1<<8,
        EarlySupporter = 1<<9,
        TeamUser = 1<<10,
        System = 1<<12,
    }
}
