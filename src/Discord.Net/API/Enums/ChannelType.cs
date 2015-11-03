namespace Discord
{
	public class ChannelType : StringEnum
	{
		/// <summary> A text-only channel. </summary>
		public static readonly ChannelType Text = new ChannelType("text");
		/// <summary> A voice-only channel. </summary>
		public static readonly ChannelType Voice = new ChannelType("voice");
		
		private ChannelType(string value)
			: base(value) { }

		public static ChannelType FromString(string value)
		{
			switch (value)
			{
				case null:
					return null;
				case "text":
					return ChannelType.Text;
				case "voice":
					return ChannelType.Voice;
				default:
					return new ChannelType(value);
			}
		}

		public static implicit operator ChannelType(string value) => FromString(value);
		public static bool operator ==(ChannelType a, ChannelType b) => a?._value == b?._value;
		public static bool operator !=(ChannelType a, ChannelType b) => a?._value != b?._value;
		public override bool Equals(object obj) => (obj as ChannelType)?._value == _value;
		public override int GetHashCode() => _value.GetHashCode();
	}
}
