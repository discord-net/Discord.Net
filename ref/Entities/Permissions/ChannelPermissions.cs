namespace Discord
{
    public struct ChannelPermissions
    {
        public static ChannelPermissions None { get; }
        public static ChannelPermissions TextOnly { get; }
        public static ChannelPermissions PrivateOnly { get; }
        public static ChannelPermissions VoiceOnly { get; }
        public static ChannelPermissions All(Channel channel) => default(ChannelPermissions);
        public static ChannelPermissions All(ChannelType channelType, bool isPrivate) => default(ChannelPermissions);

        public uint RawValue { get; }

        public bool CreateInstantInvit { get; }
        public bool ManagePermission { get; }
        public bool ManageChannel { get; }

        public bool ReadMessages { get; }
        public bool SendMessages { get; }
        public bool SendTTSMessages { get; }
        public bool ManageMessages { get; }
        public bool EmbedLinks { get; }
        public bool AttachFiles { get; }
        public bool ReadMessageHistory { get; }
        public bool MentionEveryone { get; }

        public bool Connect { get; }
        public bool Speak { get; }
        public bool MuteMembers { get; }
        public bool DeafenMembers { get; }
        public bool MoveMembers { get; }
        public bool UseVoiceActivation { get; }

        public ChannelPermissions(bool? createInstantInvite = null, bool? managePermissions = null,
            bool? manageChannel = null, bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null,
            bool? manageMessages = null, bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null,
            bool? mentionEveryone = null, bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null)
            : this()
        {
        }
        public ChannelPermissions(uint rawValue)
            : this()
        {
        }

        public ChannelPermissions Modify(ChannelPermissions basePerms, bool? createInstantInvite = null, bool? managePermissions = null,
            bool? manageChannel = null, bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null,
            bool? manageMessages = null, bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null,
            bool? mentionEveryone = null, bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null)
            => default(ChannelPermissions);
    }
}
