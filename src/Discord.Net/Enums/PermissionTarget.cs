namespace Discord
{
	public class PermissionTarget : StringEnum
	{
		/// <summary> A text-only channel. </summary>
		public static PermissionTarget Role { get; } = new PermissionTarget("role");
		/// <summary> A voice-only channel. </summary>
		public static PermissionTarget User { get; } = new PermissionTarget("member");

		private PermissionTarget(string value)
			: base(value) { }

		public static PermissionTarget FromString(string value)
		{
			switch (value)
			{
				case null:
					return null;
				case "role":
					return Role;
				case "member":
					return User;
				default:
					return new PermissionTarget(value);
			}
		}

        public static implicit operator PermissionTarget(string value) => FromString(value);
        public static bool operator ==(PermissionTarget a, PermissionTarget b) => ((object)a == null && (object)b == null) || (a?.Equals(b) ?? false);
        public static bool operator !=(PermissionTarget a, PermissionTarget b) => !(a == b);
        public override int GetHashCode() => Value.GetHashCode();
        public override bool Equals(object obj) => (obj as PermissionTarget)?.Equals(this) ?? false;
        public bool Equals(PermissionTarget type) => type != null && type.Value == Value;
    }
}
