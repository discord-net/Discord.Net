using System;

namespace Discord
{
	public class Color
	{
		public static readonly Color Default = PresetColor(0);

		public static readonly Color Cyan = PresetColor(0x1abc9c);
		public static readonly Color DarkCyan = PresetColor(0x11806a);
		public static readonly Color Green = PresetColor(0x2ecc71);
		public static readonly Color DarkGreen = PresetColor(0x1f8b4c);
		public static readonly Color Blue = PresetColor(0x3498db);
		public static readonly Color DarkBlue = PresetColor(0x206694);
		public static readonly Color Purple = PresetColor(0x9b59b6);
		public static readonly Color DarkPurple = PresetColor(0x71368a);
		public static readonly Color Red = PresetColor(0xe74c3c);
		public static readonly Color DarkRed = PresetColor(0x992d22);
		public static readonly Color Orange = PresetColor(0xe67e22);
		public static readonly Color DarkOrange = PresetColor(0xa84300);
		public static readonly Color Navy = PresetColor(0x34495e);
		public static readonly Color DarkNavy = PresetColor(0x2c3e50);
		public static readonly Color Gold = PresetColor(0xf1c40f);
		public static readonly Color DarkGold = PresetColor(0xc27c0e);

		public static readonly Color LighterGrey = PresetColor(0xbcc0c0);
		public static readonly Color LightGrey = PresetColor(0x95a5a6);
		public static readonly Color DarkGrey = PresetColor(0x979c9f);
		public static readonly Color DarkerGrey = PresetColor(0x7f8c8d);


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
		public override int GetHashCode() => _rawValue.GetHashCode();
		public override string ToString() => '#' + _rawValue.ToString("X");
	}
}
