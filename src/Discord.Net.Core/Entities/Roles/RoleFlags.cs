using System;

namespace Discord;

[Flags]
public enum RoleFlags
{
    /// <summary>
    ///     The role has no flags.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Indicates that the role can be selected by members in an onboarding.
    /// </summary>
    InPrompt = 1 << 0,
}
