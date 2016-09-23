using System;
using System.Diagnostics;

﻿namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct Color
    {
        /// <summary> Gets the default user color value. </summary>
        public static readonly Color Default = new Color(0);

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
                b;
        }
        public Color(float r, float g, float b)
        {
            RawValue =
                ((uint)(r * 255.0f) << 16) |
                ((uint)(g * 255.0f) << 8) |
                (uint)(b * 255.0f);
        }
        
        public override string ToString() =>
            $"#{Convert.ToString(RawValue, 16)}";
        private string DebuggerDisplay =>
            $"#{Convert.ToString(RawValue, 16)} ({RawValue})";
    }
}
