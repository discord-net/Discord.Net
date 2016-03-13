using System;

namespace Discord
{
    public static class EnumConverters
    {
        public static ChannelType ToChannelType(string value)
        {
            switch (value)
            {
                case "text": return ChannelType.Text;
                case "voice": return ChannelType.Voice;
                default: throw new ArgumentException("Unknown channel type", nameof(value));
            }
        }
        public static string ToString(ChannelType value)
        {
            if ((value & ChannelType.Text) != 0) return "text";
            if ((value & ChannelType.Voice) != 0) return "voice";
            throw new ArgumentException("Invalid channel tType", nameof(value));
        }

        public static PermissionTarget ToPermissionTarget(string value)
        {
            switch (value)
            {
                case "member": return PermissionTarget.User;
                case "role": return PermissionTarget.Role;
                default: throw new ArgumentException("Unknown permission target", nameof(value));
            }
        }
        public static string ToString(PermissionTarget value)
        {
            switch (value)
            {
                case PermissionTarget.User: return "member";
                case PermissionTarget.Role: return "role";
                default: throw new ArgumentException("Invalid permission target", nameof(value));
            }            
        }
    }
}
