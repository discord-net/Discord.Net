using APIChannel = Discord.API.Client.Channel;
using System.Collections.Generic;

namespace Discord
{
    public abstract class Channel : IChannel
    {
        /// <summary> An entry in a public channel's permissions that gives or takes permissions from a specific role or user. </summary>
        public class PermissionRule
        {
            /// <summary> The type of object TargetId is referring to. </summary>
            public PermissionTarget TargetType { get; }
            /// <summary> The Id of an object, whos type is specified by TargetType, that is the target of permissions being added or taken away. </summary>
            public ulong TargetId { get; }
            /// <summary> A collection of permissions that are added or taken away from the target. </summary>
            public ChannelTriStatePermissions Permissions { get; }

            internal PermissionRule(PermissionTarget targetType, ulong targetId, uint allow, uint deny)
            {
                TargetType = targetType;
                TargetId = targetId;
                Permissions = new ChannelTriStatePermissions(allow, deny);
            }
        }

        /// <summary> Gets the unique identifier for this channel. </summary>
        public ulong Id { get; }

        public abstract DiscordClient Client { get; }
        /// <summary> Gets the type of this channel. </summary>
        public abstract ChannelType Type { get; }
        public bool IsText => (Type & ChannelType.Text) != 0;
        public bool IsVoice => (Type & ChannelType.Voice) != 0;
        public bool IsPrivate => (Type & ChannelType.Private) != 0;
        public bool IsPublic => (Type & ChannelType.Public) != 0;

        public abstract User CurrentUser { get; }
        /// <summary> Gets a collection of all users in this channel. </summary>
        public abstract IEnumerable<User> Users { get; }

        internal abstract MessageManager MessageManager { get; }
        internal abstract PermissionManager PermissionManager { get; }

        protected Channel(ulong id)
        {
            Id = id;
        }

        internal abstract void Update(APIChannel model);

        internal abstract User GetUser(ulong id);

        internal abstract Channel Clone();
    }
}
