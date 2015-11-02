namespace Discord
{
	public class UserStatus : StringEnum
	{
		/// <summary> User is currently online and active. </summary>
		public static readonly UserStatus Online = new UserStatus("online");
		/// <summary> User is currently online but inactive. </summary>
		public static readonly UserStatus Idle = new UserStatus("idle");
		/// <summary> User is offline. </summary>
		public static readonly UserStatus Offline = new UserStatus("offline");

		private UserStatus(string value)
			: base(value) { }

		public static UserStatus FromString(string value)
		{
			switch (value)
			{
				case null:
					return null;
				case "online":
					return UserStatus.Online;
				case "idle":
					return UserStatus.Idle;
				case "offline":
					return UserStatus.Offline;
				default:
					return new UserStatus(value);
			}
		}

		public static implicit operator UserStatus(string value) => FromString(value);
		public static bool operator ==(UserStatus a, UserStatus b) => a?._value == b?._value;
		public static bool operator !=(UserStatus a, UserStatus b) => a?._value != b?._value;
		public override bool Equals(object obj) => (obj as UserStatus)?._value == _value;
		public override int GetHashCode() => _value.GetHashCode();
	}
}
