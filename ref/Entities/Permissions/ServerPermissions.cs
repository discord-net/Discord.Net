namespace Discord
{
    public struct ServerPermissions
    {
        public static ServerPermissions None { get; }
        public static ServerPermissions All { get; }

        public uint RawValue { get; }

        public bool CreateInstantInvite { get; }
        public bool BanMembers { get; }
        public bool KickMembers { get; }
        public bool ManageRoles { get; }
        public bool ManageChannels { get; }
        public bool ManageServer { get; }

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

        public ServerPermissions(bool? createInstantInvite = null, bool? manageRoles = null,
            bool? kickMembers = null, bool? banMembers = null, bool? manageChannel = null, bool? manageServer = null,
            bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null, bool? manageMessages = null,
            bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null, bool? mentionEveryone = null,
            bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null)
            : this()
        {
        }
        public ServerPermissions(uint rawValue)
            : this()
        {
        }

        public ServerPermissions Modify(bool? createInstantInvite = null, bool? manageRoles = null,
            bool? kickMembers = null, bool? banMembers = null, bool? manageChannel = null, bool? manageServer = null,
            bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null, bool? manageMessages = null,
            bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null, bool? mentionEveryone = null,
            bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null)
            => default(ServerPermissions);
    }
}
