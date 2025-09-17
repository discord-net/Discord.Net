using System;

namespace Discord.Interactions;

/// <summary>
///     Enum values tagged with this attribute will not be displayed as a parameter choice
/// </summary>
/// <remarks>
///     This attribute must be used along with the default <see cref="EnumConverter{T}"/>
/// </remarks>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class HideAttribute : Attribute { }
