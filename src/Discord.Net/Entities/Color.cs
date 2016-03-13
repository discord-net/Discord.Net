namespace Discord
{
	public class Color
    {
        public static readonly Color Default = new Color(0);
        
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
