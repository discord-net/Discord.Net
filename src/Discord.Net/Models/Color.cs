using System;

namespace Discord
{
	public class Color
	{
		public static readonly Color Default = PresetColor(0);

		public static readonly Color Teal = PresetColor(0x1ABC9C);
		public static readonly Color DarkTeal = PresetColor(0x11806A);
		public static readonly Color Green = PresetColor(0x2ECC71);
		public static readonly Color DarkGreen = PresetColor(0x1F8B4C);
		public static readonly Color Blue = PresetColor(0x3498DB);
		public static readonly Color DarkBlue = PresetColor(0x206694);
		public static readonly Color Purple = PresetColor(0x9B59B6);
		public static readonly Color DarkPurple = PresetColor(0x71368A);
		public static readonly Color Magenta = PresetColor(0xE91E63);
		public static readonly Color DarkMagenta = PresetColor(0xAD1457);
		public static readonly Color Gold = PresetColor(0xF1C40F);
		public static readonly Color DarkGold = PresetColor(0xC27C0E);
		public static readonly Color Orange = PresetColor(0xE67E22);
		public static readonly Color DarkOrange = PresetColor(0xA84300);
		public static readonly Color Red = PresetColor(0xE74C3C);
		public static readonly Color DarkRed = PresetColor(0x992D22);

		public static readonly Color LighterGrey = PresetColor(0x95A5A6);
		public static readonly Color DarkGrey = PresetColor(0x607D8B);
		public static readonly Color LightGrey = PresetColor(0x979C9F);
		public static readonly Color DarkerGrey = PresetColor(0x546E7A);

		private static Color PresetColor(uint packedValue)
		{
			Color color = new Color(packedValue);
			color.Lock();
			return color;
		}

		private bool _isLocked;
		private uint _rawValue;
		public uint RawValue
		{
			get { return _rawValue; }
			set
			{
				if (_isLocked)
					throw new InvalidOperationException("Unable to edit cached colors directly, use Copy() to make an editable copy.");
				_rawValue = value;
			}
		} 
		
		public Color(uint rawValue) { _rawValue = rawValue; }
		public Color(byte r, byte g, byte b) : this(((uint)r << 16) | ((uint)g << 8) | b) { }
		public Color(float r, float g, float b) : this((byte)(r * 255.0f), (byte)(g * 255.0f), (byte)(b * 255.0f)) { }

		/// <summary> Gets or sets the red component for this color. </summary>
		public byte R { get { return GetByte(3); } set { SetByte(3, value); } }
		/// <summary> Gets or sets the green component for this color. </summary>
		public byte G { get { return GetByte(2); } set { SetByte(2, value); } }
		/// <summary> Gets or sets the blue component for this color. </summary>
		public byte B { get { return GetByte(1); } set { SetByte(1, value); } }

		internal void Lock() => _isLocked = true;
		internal void SetRawValue(uint rawValue)
		{
			//Bypasses isLocked for API changes.
			_rawValue = rawValue;
		}
		protected byte GetByte(int pos) => (byte)((_rawValue >> (8 * (pos - 1))) & 0xFF);
		protected void SetByte(int pos, byte value)
		{
			if (_isLocked)
				throw new InvalidOperationException("Unable to edit cached colors directly, use Copy() to make an editable copy.");

			uint original = _rawValue;
			int bit = 8 * (pos - 1);
			uint mask = (uint)~(0xFF << bit);
            _rawValue = (_rawValue & mask) | ((uint)value << bit);
		}

		public override bool Equals(object obj) => obj is Color && (obj as Color)._rawValue == _rawValue;
		public override int GetHashCode() => unchecked(_rawValue.GetHashCode() + 1678);
		public override string ToString() => '#' + _rawValue.ToString("X");
	}
}
