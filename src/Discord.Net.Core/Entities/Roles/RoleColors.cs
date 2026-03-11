using System;

namespace Discord;

/// <summary>
///      Represents the full color configuration of a role.
/// </summary>
/// <param name="PrimaryColor">The primary (or main) color of the role.</param>
/// <param name="SecondaryColor">
///     The secondary color of the role, this will make the role a gradient between the other provided colors.
/// </param>
/// <param name="TertiaryColor">
///     The tertiary color of the role, this will turn the gradient into a holographic style.
/// </param>
public readonly record struct RoleColors(
    Color PrimaryColor,
    Color? SecondaryColor = null,
    Color? TertiaryColor = null
)
{
    private const uint HolographicPrimaryColor = 11127295;
    private const uint HolographicSecondaryColor = 16759788;
    private const uint HolographicTertiaryColor = 16761760;

    /// <summary>
    ///     A holographic <see cref="RoleColors"/>.
    /// </summary>
    public static readonly RoleColors Holographic = new(
        PrimaryColor: HolographicPrimaryColor,
        SecondaryColor: HolographicSecondaryColor,
        TertiaryColor: HolographicTertiaryColor
    );

    /// <summary>
    ///     Gets whether this color is the <see cref="Holographic"/> color.
    /// </summary>
    public bool IsHolographic => this == Holographic;

    /// <summary>
    ///     Gets whether this <see cref="RoleColors"/> is a gradient between 2 colors.
    /// </summary>
    public bool IsGradient => this is { SecondaryColor: not null, TertiaryColor: null };

    /// <summary>
    ///     Gets whether this <see cref="RoleColors"/> is a single, solid color.
    /// </summary>
    public bool IsSolidColor => this is { SecondaryColor: null, TertiaryColor: null };

    /// <summary>
    ///     When sending <see cref="TertiaryColor"/>, the API enforces the role color to be a constant value,
    ///     defined as <see cref="Holographic"/>.
    /// </summary>
    internal RoleColors Normalized
        => TertiaryColor is not null ? Holographic : this;

    /// <summary>
    ///     Creates a new <see cref="RoleColors"/> representing a single, solid color.
    /// </summary>
    /// <param name="color">The solid color to use to construct the new <see cref="RoleColors"/>.</param>
    /// <returns>
    ///     A new <see cref="RoleColors"/> representing the supplied color.
    /// </returns>
    public static RoleColors Solid(Color color)
        => new(color);

    /// <summary>
    ///     Creates a new <see cref="RoleColors"/> representing a gradient between 2 colors.
    /// </summary>
    /// <param name="primary">The primary color of the gradient.</param>
    /// <param name="secondary">The secondary color of the gradient.</param>
    /// <returns>
    ///     A new <see cref="RoleColors"/> representing the gradient between the 2 supplied colors.
    /// </returns>
    public static RoleColors Gradient(Color primary, Color secondary)
        => new(primary, secondary);

    public static implicit operator RoleColors(Color color) => Solid(color);
    public static implicit operator RoleColors?(Color? color) => color.HasValue ? Solid(color.Value) : null;
    public static implicit operator RoleColors(uint color) => Solid(color);
    public static implicit operator RoleColors?(uint? color) => color.HasValue ? Solid(color.Value) : null;
}
