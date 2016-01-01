namespace Discord
{
	public sealed class UserStatus : StringEnum
	{
		/// <summary> User is currently online and active. </summary>
		public static UserStatus Online { get; } = new UserStatus("online");
		/// <summary> User is currently online but inactive. </summary>
		public static UserStatus Idle { get; } = new UserStatus("idle");
		/// <summary> User is offline. </summary>
		public static UserStatus Offline { get; } = new UserStatus("offline");

		private UserStatus(string value)
			: base(value) { }

		public static UserStatus FromString(string value)
		{
			switch (value)
			{
				case null:
					return null;
				case "online":
					return Online;
				case "idle":
					return Idle;
				case "offline":
					return Offline;
				default:
					return new UserStatus(value);
			}
		}

		public static implicit operator UserStatus(string value) => FromString(value);
		public static bool operator ==(UserStatus a, UserStatus b) => a?.Value == b?.Value;
		public static bool operator !=(UserStatus a, UserStatus b) => a?.Value != b?.Value;
		public override bool Equals(object obj) => (obj as UserStatus)?.Value == Value;
		public override int GetHashCode() => Value.GetHashCode();
	}
}
