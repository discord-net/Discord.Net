namespace Discord
{
	public class ChannelType : StringEnum
	{
		/// <summary> A text-only channel. </summary>
		public static ChannelType Text { get; } = new ChannelType("text");
		/// <summary> A voice-only channel. </summary>
		public static ChannelType Voice { get; } = new ChannelType("voice");
		
		private ChannelType(string value)
			: base(value) { }

		public static ChannelType FromString(string value)
		{
			switch (value)
			{
				case null:
					return null;
				case "text":
					return Text;
				case "voice":
					return Voice;
				default:
					return new ChannelType(value);
			}
		}

		public static implicit operator ChannelType(string value) => FromString(value);
		public static bool operator ==(ChannelType a, ChannelType b) => a?.Value == b?.Value;
		public static bool operator !=(ChannelType a, ChannelType b) => a?.Value != b?.Value;
		public override bool Equals(object obj) => (obj as ChannelType)?.Value == Value;
		public override int GetHashCode() => Value.GetHashCode();
	}
}
