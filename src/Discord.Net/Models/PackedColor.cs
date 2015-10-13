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
					throw new InvalidOperationException("Unable to edit cached permissions directly, use Copy() to make an editable copy.");
				_rawValue = value;
			}
		} 
		
		public PackedColor(uint rawValue) { _rawValue = rawValue; }

		/// <summary> If True, a user may join channels. </summary>
		public byte Red { get { return GetByte(3); } set { SetByte(3, value); } }
		/// <summary> If True, a user may send messages. </summary>
		public byte Green { get { return GetByte(2); } set { SetByte(2, value); } }
		/// <summary> If True, a user may send text-to-speech messages. </summary>
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
				throw new InvalidOperationException("Unable to edit cached permissions directly, use Copy() to make an editable copy.");

			uint original = _rawValue;
			int bit = 8 * (pos - 1);
			uint mask = (uint)~(0xFF << bit);
            _rawValue = (_rawValue & mask) | ((uint)value << bit);
		}
	}
}
