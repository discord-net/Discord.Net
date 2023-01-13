using System;

namespace Discord;

/// <summary>
///     Represents the type of Application Role Connection Metadata.
/// </summary>
public enum RoleConnectionMetadataType
{
    /// <summary>
    /// 	The metadata's integer value is less than or equal to the guild's configured value.
    /// </summary>
    IntegerLessOrEqual = 1,

    /// <summary>
    /// 	The metadata's integer value is greater than or equal to the guild's configured value.
    /// </summary>
    IntegerGreaterOrEqual = 2,

    /// <summary>
    /// 	The metadata's integer value is equal to the guild's configured value.
    /// </summary>
    IntegerEqual = 3,

    /// <summary>
    /// 	The metadata's integer value is not equal to the guild's configured value.
    /// </summary>
    IntegerNotEqual = 4,

    /// <summary>
    /// 	The metadata's ISO8601 string value is less or equal to the guild's configured value.
    /// </summary>
    DateTimeLessOrEqual = 5,

    /// <summary>
    /// 	The metadata's ISO8601 string value is greater to the guild's configured value.
    /// </summary>
    DateTimeGreaterOrEqual = 6,

    /// <summary>
    /// 	The metadata's integer value is equal to the guild's configured value.
    /// </summary>
    BoolEqual = 7,

    /// <summary>
    /// 	The metadata's integer value is equal to the guild's configured value.
    /// </summary>
    BoolNotEqual = 8,
}
