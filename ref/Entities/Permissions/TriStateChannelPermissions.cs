namespace Discord
{
    public struct TriStateChannelPermissions
    {
        public static TriStateChannelPermissions InheritAll { get; }

        public uint AllowValue { get; }
        public uint DenyValue { get; }
        
		public PermValue CreateInstantInvite { get; }
        public PermValue ManagePermissions { get; }
        public PermValue ManageChannel { get; }
        public PermValue ReadMessages { get; }
        public PermValue SendMessages { get; }
        public PermValue SendTTSMessages { get; }
        public PermValue ManageMessages { get; }
        public PermValue EmbedLinks { get; }
        public PermValue AttachFiles { get; }
        public PermValue ReadMessageHistory { get; }
        public PermValue MentionEveryone { get; }

        public PermValue Connect { get; }
        public PermValue Speak { get; }
        public PermValue MuteMembers { get; }
        public PermValue DeafenMembers { get; }
        public PermValue MoveMembers { get; }
        public PermValue UseVoiceActivation { get; }

        public TriStateChannelPermissions(PermValue? createInstantInvite = null, PermValue? managePermissions = null,
            PermValue? manageChannel = null, PermValue? readMessages = null, PermValue? sendMessages = null, PermValue? sendTTSMessages = null,
            PermValue? manageMessages = null, PermValue? embedLinks = null, PermValue? attachFiles = null, PermValue? readMessageHistory = null,
            PermValue? mentionEveryone = null, PermValue? connect = null, PermValue? speak = null, PermValue? muteMembers = null, PermValue? deafenMembers = null,
            PermValue? moveMembers = null, PermValue? useVoiceActivation = null)
            : this()
        {
        }

        public TriStateChannelPermissions(uint allow = 0, uint deny = 0)
            : this()
        {
        }

        public TriStateChannelPermissions Modify(PermValue? createInstantInvite = null, PermValue? managePermissions = null,
            PermValue? manageChannel = null, PermValue? readMessages = null, PermValue? sendMessages = null, PermValue? sendTTSMessages = null,
            PermValue? manageMessages = null, PermValue? embedLinks = null, PermValue? attachFiles = null, PermValue? readMessageHistory = null,
            PermValue? mentionEveryone = null, PermValue? connect = null, PermValue? speak = null, PermValue? muteMembers = null, PermValue? deafenMembers = null,
            PermValue? moveMembers = null, PermValue? useVoiceActivation = null) 
            => default(TriStateChannelPermissions);
    }
}
