using System;

namespace Discord
{
	public class ChannelType : StringEnum, IEquatable<ChannelType>
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
        public static bool operator ==(ChannelType a, ChannelType b) => ((object)a == null && (object)b == null) || (a?.Equals(b) ?? false);
        public static bool operator !=(ChannelType a, ChannelType b) => !(a == b);
        public override int GetHashCode() => Value.GetHashCode();
        public override bool Equals(object obj) => (obj as ChannelType)?.Equals(this) ?? false;
        public bool Equals(ChannelType type) => type != null && type.Value == Value;
    }
}
