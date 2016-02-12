namespace Discord
{
	public class Color
    {
        public static readonly Color Default = new Color(0);

		public static readonly Color Teal = new Color(0x1ABC9C);
		public static readonly Color DarkTeal = new Color(0x11806A);
		public static readonly Color Green = new Color(0x2ECC71);
		public static readonly Color DarkGreen = new Color(0x1F8B4C);
		public static readonly Color Blue = new Color(0x3498DB);
		public static readonly Color DarkBlue = new Color(0x206694);
		public static readonly Color Purple = new Color(0x9B59B6);
		public static readonly Color DarkPurple = new Color(0x71368A);
		public static readonly Color Magenta = new Color(0xE91E63);
		public static readonly Color DarkMagenta = new Color(0xAD1457);
		public static readonly Color Gold = new Color(0xF1C40F);
		public static readonly Color DarkGold = new Color(0xC27C0E);
		public static readonly Color Orange = new Color(0xE67E22);
		public static readonly Color DarkOrange = new Color(0xA84300);
		public static readonly Color Red = new Color(0xE74C3C);
		public static readonly Color DarkRed = new Color(0x992D22);

		public static readonly Color LighterGrey = new Color(0x95A5A6);
		public static readonly Color DarkGrey = new Color(0x607D8B);
		public static readonly Color LightGrey = new Color(0x979C9F);
		public static readonly Color DarkerGrey = new Color(0x546E7A);
        
		public uint RawValue { get; }
		
		public Color(uint rawValue) { RawValue = rawValue; }
		public Color(byte r, byte g, byte b) : this(((uint)r << 16) | ((uint)g << 8) | b) { }
		public Color(float r, float g, float b) : this((byte)(r * 255.0f), (byte)(g * 255.0f), (byte)(b * 255.0f)) { }

		/// <summary> Gets or sets the red component for this color. </summary>
		public byte R => (byte)(RawValue >> 16);
        /// <summary> Gets or sets the green component for this color. </summary>
        public byte G => (byte)(RawValue >> 8);
        /// <summary> Gets or sets the blue component for this color. </summary>
        public byte B => (byte)(RawValue);

        private byte GetByte(int pos) => (byte)(RawValue >> (8 * (pos - 1)));

        public override string ToString() => '#' + RawValue.ToString("X");
    }
}
