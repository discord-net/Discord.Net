using System;

namespace Discord
{
	public class PackedColor
	{
		public static readonly PackedColor Default = PresetColor(0);

		public static readonly PackedColor Aqua = PresetColor(1752220);
		public static readonly PackedColor DarkAqua = PresetColor(1146986);
		public static readonly PackedColor Green = PresetColor(3066993);
		public static readonly PackedColor DarkGreen = PresetColor(2067276);
		public static readonly PackedColor Blue = PresetColor(3447003);
		public static readonly PackedColor DarkBlue = PresetColor(2123412);
		public static readonly PackedColor Purple = PresetColor(10181046);
		public static readonly PackedColor DarkPurple = PresetColor(7419530);
		public static readonly PackedColor Gold = PresetColor(15844367);
		public static readonly PackedColor DarkGold = PresetColor(12745742);
		public static readonly PackedColor Orange = PresetColor(15105570);
		public static readonly PackedColor DarkOrange = PresetColor(11027200);
		public static readonly PackedColor Red = PresetColor(15158332);
		public static readonly PackedColor DarkRed = PresetColor(10038562);
		public static readonly PackedColor Navy = PresetColor(3426654);
		public static readonly PackedColor DarkNavy = PresetColor(2899536);

		public static readonly PackedColor LighterGrey = PresetColor(12370112);
		public static readonly PackedColor LightGrey = PresetColor(9807270);
		public static readonly PackedColor DarkGrey = PresetColor(9936031);
		public static readonly PackedColor DarkerGrey = PresetColor(8359053);

		private static PackedColor PresetColor(uint packedValue)
		{
			PackedColor color = new PackedColor(packedValue);
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
		
		public PackedColor(uint rawValue) { _rawValue = rawValue; }
		public PackedColor(byte r, byte g, byte b) : this(((uint)r << 16) | ((uint)g << 8) | b) { }
		public PackedColor(float r, float g, float b) : this((byte)(r * 255.0f), (byte)(g * 255.0f), (byte)(b * 255.0f)) { }

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
	}
}
