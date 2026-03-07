using System;

namespace Discord
{
    /// <summary>
    ///     Represents the full color configuration of a role.
    /// </summary>
    public struct RoleColors : IEquatable<RoleColors>
    {
        /// <summary>
        ///     Gets the primary color required by Discord for holographic role colors.
        /// </summary>
        public static Color HolographicPrimaryColor { get; } = new(11127295);

        /// <summary>
        ///     Gets the secondary color required by Discord for holographic role colors.
        /// </summary>
        public static Color HolographicSecondaryColor { get; } = new(16759788);

        /// <summary>
        ///     Gets the tertiary color required by Discord for holographic role colors.
        /// </summary>
        public static Color HolographicTertiaryColor { get; } = new(16761760);

        /// <summary>
        ///     Gets the primary color of this role.
        /// </summary>
        public Color? PrimaryColor { get; }

        /// <summary>
        ///     Gets the secondary color of this role.
        /// </summary>
        public Color? SecondaryColor { get; }

        /// <summary>
        ///     Gets the tertiary color of this role.
        /// </summary>
        public Color? TertiaryColor { get; }

        /// <summary>
        ///     Initializes a new <see cref="RoleColors"/> struct with the given role colors.
        /// </summary>
        public RoleColors(Color? primaryColor = null, Color? secondaryColor = null, Color? tertiaryColor = null)
        {
            PrimaryColor = primaryColor;
            SecondaryColor = secondaryColor;
            TertiaryColor = tertiaryColor;
        }

        /// <summary>
        ///     Creates a <see cref="RoleColors"/> value from a single solid role color.
        /// </summary>
        public static RoleColors FromColor(Color color)
            => new(color);

        /// <summary>
        ///     Returns the role colors normalized to Discord's API requirements.
        /// </summary>
        public RoleColors Normalize()
            => TertiaryColor.HasValue
                ? new(HolographicPrimaryColor, HolographicSecondaryColor, HolographicTertiaryColor)
                : this;

        public static bool operator ==(RoleColors left, RoleColors right)
            => left.Equals(right);

        public static bool operator !=(RoleColors left, RoleColors right)
            => !left.Equals(right);

        public bool Equals(RoleColors other)
            => PrimaryColor == other.PrimaryColor
            && SecondaryColor == other.SecondaryColor
            && TertiaryColor == other.TertiaryColor;

        public override bool Equals(object obj)
            => obj is RoleColors other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(PrimaryColor, SecondaryColor, TertiaryColor);

        public override string ToString()
            => $"{PrimaryColor} / {SecondaryColor?.ToString() ?? "null"} / {TertiaryColor?.ToString() ?? "null"}";
    }
}
