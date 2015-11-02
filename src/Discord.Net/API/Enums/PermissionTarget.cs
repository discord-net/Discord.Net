namespace Discord
{
	public class PermissionTarget : StringEnum
	{
		/// <summary> A text-only channel. </summary>
		public static readonly PermissionTarget Role = new PermissionTarget("role");
		/// <summary> A voice-only channel. </summary>
		public static readonly PermissionTarget User = new PermissionTarget("member");

		private PermissionTarget(string value)
			: base(value) { }

		public static PermissionTarget FromString(string value)
		{
			switch (value)
			{
				case null:
					return null;
				case "role":
					return PermissionTarget.Role;
				case "member":
					return PermissionTarget.User;
				default:
					return new PermissionTarget(value);
			}
		}

		public static implicit operator PermissionTarget(string value) => FromString(value);
		public static bool operator ==(PermissionTarget a, PermissionTarget b) => a?._value == b?._value;
		public static bool operator !=(PermissionTarget a, PermissionTarget b) => a?._value != b?._value;
		public override bool Equals(object obj) => (obj as PermissionTarget)?._value == _value;
		public override int GetHashCode() => _value.GetHashCode();
	}
}
