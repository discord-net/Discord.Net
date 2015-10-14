using System;

namespace Discord
{
	public class PackedColor
	{
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

		/// <summary> Gets or sets the red component for this color. </summary>
		public byte Red { get { return GetByte(3); } set { SetByte(3, value); } }
		/// <summary> Gets or sets the green component for this color. </summary>
		public byte Green { get { return GetByte(2); } set { SetByte(2, value); } }
		/// <summary> Gets or sets the blue component for this color. </summary>
		public byte Blue { get { return GetByte(1); } set { SetByte(1, value); } }

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
