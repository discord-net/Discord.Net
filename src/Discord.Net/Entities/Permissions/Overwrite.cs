using Model = Discord.API.Overwrite;

namespace Discord
{
    public struct Overwrite
    {
        /// <summary> Gets the unique identifier for the object this overwrite is targeting. </summary>
        public ulong TargetId { get; }
        /// <summary> Gets the type of object this overwrite is targeting. </summary>
        public PermissionTarget TargetType { get; }
        /// <summary> Gets the permissions associated with this overwrite entry. </summary>
        public OverwritePermissions Permissions { get; }

        /// <summary> Creates a new Overwrite with provided target information and modified permissions. </summary>
        internal Overwrite(Model model)
        {
            TargetId = model.TargetId;
            TargetType = model.TargetType;
            Permissions = new OverwritePermissions(model.Allow, model.Deny);
        }
    }
}
