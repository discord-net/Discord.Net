using System;
using System.Diagnostics;
#if NETSTANDARD2_0 || NET45
using StandardColor = System.Drawing.Color;
#endif

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct Color
    {
        /// <summary> Gets the default user color value. </summary>
        public static readonly Color Default = new Color(0);
        /// <summary> Gets the teal color value </summary>
        public static readonly Color Teal = new Color(0x1ABC9C);
        /// <summary> Gets the dark teal color value </summary>
        public static readonly Color DarkTeal = new Color(0x11806A);
        /// <summary> Gets the green color value </summary>
        public static readonly Color Green = new Color(0x2ECC71);
        /// <summary> Gets the dark green color value </summary>
        public static readonly Color DarkGreen = new Color(0x1F8B4C);
        /// <summary> Gets the blue color value </summary>
        public static readonly Color Blue = new Color(0x3498DB);
        /// <summary> Gets the dark blue color value </summary>
        public static readonly Color DarkBlue = new Color(0x206694);
        /// <summary> Gets the purple color value </summary>
        public static readonly Color Purple = new Color(0x9B59B6);
        /// <summary> Gets the dark purple color value </summary>
        public static readonly Color DarkPurple = new Color(0x71368A);
        /// <summary> Gets the magenta color value </summary>
        public static readonly Color Magenta = new Color(0xE91E63);
        /// <summary> Gets the dark magenta color value </summary>
        public static readonly Color DarkMagenta = new Color(0xAD1457);
        /// <summary> Gets the gold color value </summary>
        public static readonly Color Gold = new Color(0xF1C40F);
        /// <summary> Gets the light orange color value </summary>
        public static readonly Color LightOrange = new Color(0xC27C0E);
        /// <summary> Gets the orange color value </summary>
        public static readonly Color Orange = new Color(0xE67E22);
        /// <summary> Gets the dark orange color value </summary>
        public static readonly Color DarkOrange = new Color(0xA84300);
        /// <summary> Gets the red color value </summary>
        public static readonly Color Red = new Color(0xE74C3C);
        /// <summary> Gets the dark red color value </summary>
        public static readonly Color DarkRed = new Color(0x992D22); 
        /// <summary> Gets the light grey color value </summary>
        public static readonly Color LightGrey = new Color(0x979C9F);
        /// <summary> Gets the lighter grey color value </summary>
        public static readonly Color LighterGrey = new Color(0x95A5A6);
        /// <summary> Gets the dark grey color value </summary>
        public static readonly Color DarkGrey = new Color(0x607D8B);
        /// <summary> Gets the darker grey color value </summary>
        public static readonly Color DarkerGrey = new Color(0x546E7A);

        /// <summary> Gets the encoded value for this color. </summary>
        public uint RawValue { get; }

        /// <summary> Gets the red component for this color. </summary>
        public byte R => (byte)(RawValue >> 16);
        /// <summary> Gets the green component for this color. </summary>
        public byte G => (byte)(RawValue >> 8);
        /// <summary> Gets the blue component for this color. </summary>
        public byte B => (byte)(RawValue);

        public Color(uint rawValue)
        {
            RawValue = rawValue;
        }
        public Color(byte r, byte g, byte b)
        {
            RawValue =
                ((uint)r << 16) |
                ((uint)g << 8) |
                (uint)b;
        }
        public Color(int r, int g, int b)
        {
            if (r < 0 || r > 255)
                throw new ArgumentOutOfRangeException(nameof(r), "Value must be within [0,255]");
            if (g < 0 || g > 255)
                throw new ArgumentOutOfRangeException(nameof(g), "Value must be within [0,255]");
            if (b < 0 || b > 255)
                throw new ArgumentOutOfRangeException(nameof(b), "Value must be within [0,255]");
            RawValue =
                ((uint)r << 16) |
                ((uint)g << 8) |
                (uint)b;
        }
        public Color(float r, float g, float b)
        {
            if (r < 0.0f || r > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(r), "Value must be within [0,1]");
            if (g < 0.0f || g > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(g), "Value must be within [0,1]");
            if (b < 0.0f || b > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(b), "Value must be within [0,1]");
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

        public override string ToString() =>
            $"#{Convert.ToString(RawValue, 16)}";
        private string DebuggerDisplay =>
            $"#{Convert.ToString(RawValue, 16)} ({RawValue})";
    }
}
