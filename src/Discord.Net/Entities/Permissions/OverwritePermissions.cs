using System;

namespace Discord
{
    public struct OverwritePermissions
    {
        /// <summary> Gets a blank OverwritePermissions that inherits all permissions. </summary>
        public static OverwritePermissions InheritAll { get; } = new OverwritePermissions();
        /// <summary> Gets a OverwritePermissions that grants all permissions for a given channelType. </summary>
        public static OverwritePermissions AllowAll(ChannelType channelType) 
            => new OverwritePermissions(ChannelPermissions.All(channelType).RawValue, 0);
        /// <summary> Gets a OverwritePermissions that denies all permissions for a given channelType. </summary>
        public static OverwritePermissions DenyAll(ChannelType channelType)
            => new OverwritePermissions(0, ChannelPermissions.All(channelType).RawValue);

        /// <summary> Gets a packed value representing all the allowed permissions in this OverwritePermissions. </summary>
        public uint AllowValue { get; }
        /// <summary> Gets a packed value representing all the denied permissions in this OverwritePermissions. </summary>
        public uint DenyValue { get; }

        /// <summary> If True, a user may create invites. </summary>
        public PermValue CreateInstantInvite => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.CreateInstantInvite);
        /// <summary> If True, a user may adjust permissions. This also implictly grants all other permissions. </summary>
        public PermValue ManagePermissions => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.ManageRolesOrPermissions);
        /// <summary> If True, a user may create, delete and modify this channel. </summary>
        public PermValue ManageChannel => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.ManageChannel);
        /// <summary> If True, a user may join channels. </summary>
        public PermValue ReadMessages => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.ReadMessages);
        /// <summary> If True, a user may send messages. </summary>
        public PermValue SendMessages => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.SendMessages);
        /// <summary> If True, a user may send text-to-speech messages. </summary>
        public PermValue SendTTSMessages => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.SendTTSMessages);
        /// <summary> If True, a user may delete messages. </summary>
        public PermValue ManageMessages => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.ManageMessages);
        /// <summary> If True, Discord will auto-embed links sent by this user. </summary>
        public PermValue EmbedLinks => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.EmbedLinks);
        /// <summary> If True, a user may send files. </summary>
        public PermValue AttachFiles => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.AttachFiles);
        /// <summary> If True, a user may read previous messages. </summary>
        public PermValue ReadMessageHistory => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.ReadMessageHistory);
        /// <summary> If True, a user may mention @everyone. </summary>
        public PermValue MentionEveryone => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.MentionEveryone);

        /// <summary> If True, a user may connect to a voice channel. </summary>
        public PermValue Connect => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.Connect);
        /// <summary> If True, a user may speak in a voice channel. </summary>
        public PermValue Speak => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.Speak);
        /// <summary> If True, a user may mute users. </summary>
        public PermValue MuteMembers => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.MuteMembers);
        /// <summary> If True, a user may deafen users. </summary>
        public PermValue DeafenMembers => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.DeafenMembers);
        /// <summary> If True, a user may move other users between voice channels. </summary>
        public PermValue MoveMembers => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.MoveMembers);
        /// <summary> If True, a user may use voice activation rather than push-to-talk. </summary>
        public PermValue UseVoiceActivation => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBit.UseVoiceActivation);

        /// <summary> Creates a new OverwritePermissions with the provided allow and deny packed values. </summary>
        public OverwritePermissions(uint allowValue, uint denyValue)
        {
            AllowValue = allowValue;
            DenyValue = denyValue;
        }

        private OverwritePermissions(uint allowValue, uint denyValue, PermValue? createInstantInvite = null, PermValue? managePermissions = null,
            PermValue? manageChannel = null, PermValue? readMessages = null, PermValue? sendMessages = null, PermValue? sendTTSMessages = null,
            PermValue? manageMessages = null, PermValue? embedLinks = null, PermValue? attachFiles = null, PermValue? readMessageHistory = null,
            PermValue? mentionEveryone = null, PermValue? connect = null, PermValue? speak = null, PermValue? muteMembers = null, PermValue? deafenMembers = null,
            PermValue? moveMembers = null, PermValue? useVoiceActivation = null)
        {
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, createInstantInvite, PermissionBit.CreateInstantInvite);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, managePermissions, PermissionBit.ManageRolesOrPermissions);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, manageChannel, PermissionBit.ManageChannel);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, readMessages, PermissionBit.ReadMessages);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, sendMessages, PermissionBit.SendMessages);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, sendTTSMessages, PermissionBit.SendTTSMessages);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, manageMessages, PermissionBit.ManageMessages);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, embedLinks, PermissionBit.EmbedLinks);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, attachFiles, PermissionBit.AttachFiles);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, readMessageHistory, PermissionBit.ReadMessageHistory);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, mentionEveryone, PermissionBit.MentionEveryone);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, connect, PermissionBit.Connect);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, speak, PermissionBit.Speak);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, muteMembers, PermissionBit.MuteMembers);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, deafenMembers, PermissionBit.DeafenMembers);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, moveMembers, PermissionBit.MoveMembers);
            PermissionsHelper.SetValue(ref allowValue, ref denyValue, useVoiceActivation, PermissionBit.UseVoiceActivation);

            AllowValue = allowValue;
            DenyValue = denyValue;
        }

        /// <summary> Creates a new ChannelPermissions with the provided permissions. </summary>
        public OverwritePermissions(PermValue createInstantInvite = PermValue.Inherit, PermValue managePermissions = PermValue.Inherit,
            PermValue manageChannel = PermValue.Inherit, PermValue readMessages = PermValue.Inherit, PermValue sendMessages = PermValue.Inherit, PermValue sendTTSMessages = PermValue.Inherit,
            PermValue manageMessages = PermValue.Inherit, PermValue embedLinks = PermValue.Inherit, PermValue attachFiles = PermValue.Inherit, PermValue readMessageHistory = PermValue.Inherit,
            PermValue mentionEveryone = PermValue.Inherit, PermValue connect = PermValue.Inherit, PermValue speak = PermValue.Inherit, PermValue muteMembers = PermValue.Inherit, PermValue deafenMembers = PermValue.Inherit,
            PermValue moveMembers = PermValue.Inherit, PermValue useVoiceActivation = PermValue.Inherit)
            : this(0, 0, createInstantInvite, managePermissions, manageChannel, readMessages, sendMessages, sendTTSMessages,
                  manageMessages, embedLinks, attachFiles, readMessageHistory, mentionEveryone, connect, speak, muteMembers, deafenMembers, moveMembers, useVoiceActivation) { }

        /// <summary> Creates a new OverwritePermissions from this one, changing the provided non-null permissions. </summary>
        public OverwritePermissions Modify(PermValue? createInstantInvite = null, PermValue? managePermissions = null,
            PermValue? manageChannel = null, PermValue? readMessages = null, PermValue? sendMessages = null, PermValue? sendTTSMessages = null,
            PermValue? manageMessages = null, PermValue? embedLinks = null, PermValue? attachFiles = null, PermValue? readMessageHistory = null,
            PermValue? mentionEveryone = null, PermValue? connect = null, PermValue? speak = null, PermValue? muteMembers = null, PermValue? deafenMembers = null,
            PermValue? moveMembers = null, PermValue? useVoiceActivation = null)
            => new OverwritePermissions(AllowValue, DenyValue, createInstantInvite, managePermissions, manageChannel, readMessages, sendMessages, sendTTSMessages,
                  manageMessages, embedLinks, attachFiles, readMessageHistory, mentionEveryone, connect, speak, muteMembers, deafenMembers, moveMembers, useVoiceActivation);

        /// <inheritdoc />
        public override string ToString() => $"Allow: {Convert.ToString(AllowValue, 2)}, Deny: {Convert.ToString(DenyValue, 2)}";
    }
}
