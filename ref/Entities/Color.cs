namespace Discord
{
	public class Color
    {
        public static readonly Color Default = new Color(0);
        
		public uint RawValue { get; }
		
		public Color(uint rawValue) { }
        public Color(byte r, byte g, byte b) { }
        public Color(float r, float g, float b) { }
        
		public byte R { get; }
        public byte G { get; }
        public byte B { get; }
    }
}
