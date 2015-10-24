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
	}
}
