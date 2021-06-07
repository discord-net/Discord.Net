using System;
using System.Diagnostics;
using StandardColor = System.Drawing.Color;

namespace Discord.Net
{
    /// <summary>
    /// Represents a RGB (red, green, blue) color.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct Color
    {
        #region Colors
        /// <summary>
        /// Gets the default user color value.
        /// </summary>
        public static readonly Color Default = new(0);

        /// <summary>
        /// Gets the teal color value.
        /// </summary>
        public static readonly Color Teal = new(0x1ABC9C);

        /// <summary>
        /// Gets the dark teal color value.
        /// </summary>
        public static readonly Color DarkTeal = new(0x11806A);

        /// <summary>
        /// Gets the green color value.
        /// </summary>
        public static readonly Color Green = new(0x2ECC71);

        /// <summary>
        /// Gets the dark green color value.
        /// </summary>
        public static readonly Color DarkGreen = new(0x1F8B4C);

        /// <summary>
        /// Gets the blue color value.
        /// </summary>
        public static readonly Color Blue = new(0x3498DB);

        /// <summary>
        /// Gets the dark blue color value.
        /// </summary>
        public static readonly Color DarkBlue = new(0x206694);

        /// <summary>
        /// Gets the purple color value.
        /// </summary>
        public static readonly Color Purple = new(0x9B59B6);

        /// <summary>
        /// Gets the dark purple color value.
        /// </summary>
        public static readonly Color DarkPurple = new(0x71368A);

        /// <summary>
        /// Gets the magenta color value.
        /// </summary>
        public static readonly Color Magenta = new(0xE91E63);

        /// <summary>
        /// Gets the dark magenta color value.
        /// </summary>
        public static readonly Color DarkMagenta = new(0xAD1457);

        /// <summary>
        /// Gets the gold color value.
        /// </summary>
        public static readonly Color Gold = new(0xF1C40F);

        /// <summary>
        /// Gets the light orange color value.
        /// </summary>
        public static readonly Color LightOrange = new(0xC27C0E);

        /// <summary>
        /// Gets the orange color value.
        /// </summary>
        public static readonly Color Orange = new(0xE67E22);

        /// <summary>
        /// Gets the dark orange color value.
        /// </summary>
        public static readonly Color DarkOrange = new(0xA84300);

        /// <summary>
        /// Gets the red color value.
        /// </summary>
        public static readonly Color Red = new(0xE74C3C);

        /// <summary>
        /// Gets the dark red color value.
        /// </summary>
        public static readonly Color DarkRed = new(0x992D22);

        /// <summary>
        /// Gets the light grey color value.
        /// </summary>
        public static readonly Color LightGrey = new(0x979C9F);

        /// <summary>
        /// Gets the lighter grey color value.
        /// </summary>
        public static readonly Color LighterGrey = new(0x95A5A6);

        /// <summary>
        /// Gets the dark grey color value.
        /// </summary>
        public static readonly Color DarkGrey = new(0x607D8B);

        /// <summary>
        /// Gets the darker grey color value.
        /// </summary>
        public static readonly Color DarkerGrey = new(0x546E7A);

        #endregion Colors

        /// <summary>
        /// Gets the encoded value for this color.
        /// </summary>
        public uint RawValue { get; }

        /// <summary>
        /// Gets the red component for this color.
        /// </summary>
        public byte R => (byte)(RawValue >> 16);

        /// <summary>
        /// Gets the green component for this color.
        /// </summary>
        public byte G => (byte)(RawValue >> 8);

        /// <summary>
        /// Gets the blue component for this color.
        /// </summary>
        public byte B => (byte)(RawValue);

        /// <summary>
        /// Creates a <see cref="Color"/> based on an encoded value.
        /// </summary>
        /// <param name="rawValue">
        /// Color encoded value.
        /// </param>
        public Color(uint rawValue)
        {
            RawValue = rawValue;
        }

        /// <summary>
        /// Creates a <see cref="Color"/> based on the RGB color provided by <see cref="byte"/>s.
        /// </summary>
        /// <param name="red">
        /// Red color.
        /// </param>
        /// <param name="green">
        /// Green color.
        /// </param>
        /// <param name="blue">
        /// Blue color.
        /// </param>
        public Color(byte red, byte green, byte blue)
        {
            RawValue = ((uint)red << 16) | ((uint)green << 8) | blue;
        }

        /// <summary>
        /// Creates a <see cref="Color"/> based on the RGB color provided by <see cref="int"/>s.
        /// </summary>
        /// <param name="red">
        /// Red color.
        /// </param>
        /// <param name="green">
        /// Green color.
        /// </param>
        /// <param name="blue">
        /// Blue color.
        /// </param>
        public Color(int red, int green, int blue)
        {
            if (red < 0 || red > 255)
                throw new ArgumentOutOfRangeException(nameof(red), "Value must be within [0,255]");
            if (green < 0 || green > 255)
                throw new ArgumentOutOfRangeException(nameof(green), "Value must be within [0,255]");
            if (blue < 0 || blue > 255)
                throw new ArgumentOutOfRangeException(nameof(blue), "Value must be within [0,255]");
            RawValue = ((uint)red << 16) | ((uint)green << 8) | (uint)blue;
        }

        /// <summary>
        /// Creates a <see cref="Color"/> based on the RGB color provided by <see cref="float"/>s.
        /// </summary>
        /// <param name="red">
        /// Red color.
        /// </param>
        /// <param name="green">
        /// Green color.
        /// </param>
        /// <param name="blue">
        /// Blue color.
        /// </param>
        public Color(float red, float green, float blue)
        {
            if (red < 0.0f || red > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(red), "Value must be within [0,1]");
            if (green < 0.0f || green > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(green), "Value must be within [0,1]");
            if (blue < 0.0f || blue > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(blue), "Value must be within [0,1]");
            RawValue =
                ((uint)(red * 255.0f) << 16) |
                ((uint)(green * 255.0f) << 8) |
                (uint)(blue * 255.0f);
        }

        /// <summary>
        /// Implicitly converts this <see cref="Color"/> to <see cref="StandardColor"/>.
        /// </summary>
        /// <param name="color">
        /// <see cref="Color"/> to convert.
        /// </param>
        public static implicit operator StandardColor(Color color)
            => StandardColor.FromArgb((int)color.RawValue);

        /// <summary>
        /// Explicitly converts a <see cref="StandardColor"/> to a <see cref="Color"/>.
        /// </summary>
        /// <param name="color">
        /// <see cref="StandardColor"/> to convert.
        /// </param>
        public static explicit operator Color(StandardColor color)
            => new((uint)color.ToArgb() << 8 >> 8);

        /// <summary>
        /// Returns the encoded raw value as <see cref="string"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"#{Convert.ToString(RawValue, 16)}";

        private string DebuggerDisplay
            => $"#{Convert.ToString(RawValue, 16)} ({RawValue})";
    }
}
