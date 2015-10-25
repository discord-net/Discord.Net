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
	}
}
