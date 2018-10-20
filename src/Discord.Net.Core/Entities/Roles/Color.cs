using System;
using System.Diagnostics;
#if NETSTANDARD2_0 || NET45
using StandardColor = System.Drawing.Color;
#endif

namespace Discord
{
    /// <summary>
    ///     Represents a color used in Discord.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct Color
    {
        /// <summary> Gets the default user color value. </summary>
        public static readonly Color Default = new Color(0);
        /// <summary> Gets the teal color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/1ABC9C">1ABC9C</see>.</returns>
        public static readonly Color Teal = new Color(0x1ABC9C);
        /// <summary> Gets the dark teal color value. </summary>
        public static readonly Color DarkTeal = new Color(0x11806A);
        /// <summary> Gets the green color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/11806A">11806A</see>.</returns>
        public static readonly Color Green = new Color(0x2ECC71);
        /// <summary> Gets the dark green color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/2ECC71">2ECC71</see>.</returns>
        public static readonly Color DarkGreen = new Color(0x1F8B4C);
        /// <summary> Gets the blue color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/1F8B4C">1F8B4C</see>.</returns>
        public static readonly Color Blue = new Color(0x3498DB);
        /// <summary> Gets the dark blue color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/3498DB">3498DB</see>.</returns>
        public static readonly Color DarkBlue = new Color(0x206694);
        /// <summary> Gets the purple color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/206694">206694</see>.</returns>
        public static readonly Color Purple = new Color(0x9B59B6);
        /// <summary> Gets the dark purple color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/9B59B6">9B59B6</see>.</returns>
        public static readonly Color DarkPurple = new Color(0x71368A);
        /// <summary> Gets the magenta color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/71368A">71368A</see>.</returns>
        public static readonly Color Magenta = new Color(0xE91E63);
        /// <summary> Gets the dark magenta color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/E91E63">E91E63</see>.</returns>
        public static readonly Color DarkMagenta = new Color(0xAD1457);
        /// <summary> Gets the gold color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/AD1457">AD1457</see>.</returns>
        public static readonly Color Gold = new Color(0xF1C40F);
        /// <summary> Gets the light orange color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/F1C40F">F1C40F</see>.</returns>
        public static readonly Color LightOrange = new Color(0xC27C0E);
        /// <summary> Gets the orange color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/C27C0E">C27C0E</see>.</returns>
        public static readonly Color Orange = new Color(0xE67E22);
        /// <summary> Gets the dark orange color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/E67E22">E67E22</see>.</returns>
        public static readonly Color DarkOrange = new Color(0xA84300);
        /// <summary> Gets the red color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/A84300">A84300</see>.</returns>
        public static readonly Color Red = new Color(0xE74C3C);
        /// <summary> Gets the dark red color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/E74C3C">E74C3C</see>.</returns>
        public static readonly Color DarkRed = new Color(0x992D22); 
        /// <summary> Gets the light grey color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/992D22">992D22</see>.</returns>
        public static readonly Color LightGrey = new Color(0x979C9F);
        /// <summary> Gets the lighter grey color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/979C9F">979C9F</see>.</returns>
        public static readonly Color LighterGrey = new Color(0x95A5A6);
        /// <summary> Gets the dark grey color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/95A5A6">95A5A6</see>.</returns>
        public static readonly Color DarkGrey = new Color(0x607D8B);
        /// <summary> Gets the darker grey color value. </summary>
        /// <returns> A color struct with the hex value of <see href="http://www.color-hex.com/color/607D8B">607D8B</see>.</returns>
        public static readonly Color DarkerGrey = new Color(0x546E7A);

        /// <summary> Gets the encoded value for this color. </summary>
        /// <remarks>
        ///     This value is encoded as an unsigned integer value. The most-significant 8 bits contain the red value,
        ///     the middle 8 bits contain the green value, and the least-significant 8 bits contain the blue value.
        /// </remarks>
        public uint RawValue { get; }

        /// <summary> Gets the red component for this color. </summary>
        public byte R => (byte)(RawValue >> 16);
        /// <summary> Gets the green component for this color. </summary>
        public byte G => (byte)(RawValue >> 8);
        /// <summary> Gets the blue component for this color. </summary>
        public byte B => (byte)(RawValue);

        /// <summary>
        ///     Initializes a <see cref="Color"/> struct with the given raw value.
        /// </summary>
        /// <example>
        ///     The following will create a color that has a hex value of 
        ///     <see href="http://www.color-hex.com/color/607d8b">#607D8B</see>.
        ///     <code language="cs">
        ///     Color darkGrey = new Color(0x607D8B);
        ///     </code>
        /// </example>
        /// <param name="rawValue">The raw value of the color (e.g. <c>0x607D8B</c>).</param>
        public Color(uint rawValue)
        {
            RawValue = rawValue;
        }
        /// <summary>
        ///     Initializes a <see cref="Color" /> struct with the given RGB bytes.
        /// </summary>
        /// <example>
        ///     The following will create a color that has a value of 
        ///     <see href="http://www.color-hex.com/color/607d8b">#607D8B</see>.
        ///     <code language="cs">
        ///     Color darkGrey = new Color((byte)0b_01100000, (byte)0b_01111101, (byte)0b_10001011);
        ///     </code>
        /// </example>
        /// <param name="r">The byte that represents the red color.</param>
        /// <param name="g">The byte that represents the green color.</param>
        /// <param name="b">The byte that represents the blue color.</param>
        public Color(byte r, byte g, byte b)
        {
            RawValue =
                ((uint)r << 16) |
                ((uint)g << 8) |
                (uint)b;
        }

        /// <summary>
        ///     Initializes a <see cref="Color"/> struct with the given RGB value.
        /// </summary>
        /// <example>
        ///     The following will create a color that has a value of 
        ///     <see href="http://www.color-hex.com/color/607d8b">#607D8B</see>.
        ///     <code language="cs">
        ///     Color darkGrey = new Color(96, 125, 139);
        ///     </code>
        /// </example>
        /// <param name="r">The value that represents the red color. Must be within 0~255.</param>
        /// <param name="g">The value that represents the green color. Must be within 0~255.</param>
        /// <param name="b">The value that represents the blue color. Must be within 0~255.</param>
        /// <exception cref="ArgumentOutOfRangeException">The argument value is not between 0 to 255.</exception>
        public Color(int r, int g, int b)
        {
            if (r < 0 || r > 255)
                throw new ArgumentOutOfRangeException(nameof(r), "Value must be within [0,255].");
            if (g < 0 || g > 255)
                throw new ArgumentOutOfRangeException(nameof(g), "Value must be within [0,255].");
            if (b < 0 || b > 255)
                throw new ArgumentOutOfRangeException(nameof(b), "Value must be within [0,255].");
            RawValue =
                ((uint)r << 16) |
                ((uint)g << 8) |
                (uint)b;
        }
        /// <summary>
        ///     Initializes a <see cref="Color"/> struct with the given RGB float value.
        /// </summary>
        /// <example>
        ///     The following will create a color that has a value of 
        ///     <see href="http://www.color-hex.com/color/607c8c">#607c8c</see>.
        ///     <code language="cs">
        ///     Color darkGrey = new Color(0.38f, 0.49f, 0.55f);
        ///     </code>
        /// </example>
        /// <param name="r">The value that represents the red color. Must be within 0~1.</param>
        /// <param name="g">The value that represents the green color. Must be within 0~1.</param>
        /// <param name="b">The value that represents the blue color. Must be within 0~1.</param>
        /// <exception cref="ArgumentOutOfRangeException">The argument value is not between 0 to 1.</exception>
        public Color(float r, float g, float b)
        {
            if (r < 0.0f || r > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(r), "Value must be within [0,1].");
            if (g < 0.0f || g > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(g), "Value must be within [0,1].");
            if (b < 0.0f || b > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(b), "Value must be within [0,1].");
            RawValue =
                ((uint)(r * 255.0f) << 16) |
                ((uint)(g * 255.0f) << 8) |
                (uint)(b * 255.0f);
        }

        public static bool operator ==(Color lhs, Color rhs)
            => lhs.RawValue == rhs.RawValue;

        public static bool operator !=(Color lhs, Color rhs)
            => lhs.RawValue != rhs.RawValue;

        public override bool Equals(object obj)
            => (obj is Color c && RawValue == c.RawValue);

        public override int GetHashCode() => RawValue.GetHashCode();

#if NETSTANDARD2_0 || NET45
        public static implicit operator StandardColor(Color color) =>
            StandardColor.FromArgb((int)color.RawValue);
        public static explicit operator Color(StandardColor color) =>
            new Color((uint)color.ToArgb() << 8 >> 8);
#endif

        /// <summary>
        ///     Gets the hexadecimal representation of the color (e.g. <c>#000ccc</c>).
        /// </summary>
        /// <returns>
        ///     A hexadecimal string of the color.
        /// </returns>
        public override string ToString() =>
            $"#{Convert.ToString(RawValue, 16)}";
        private string DebuggerDisplay =>
            $"#{Convert.ToString(RawValue, 16)} ({RawValue})";
    }
}
