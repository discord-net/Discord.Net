namespace Discord
{
    public enum ChannelPermission : ulong
    {
        CREATE_INSTANT_INVITE =     0x00000001,
        MANAGE_CHANNELS =           0x00000010,
        
        ADD_REACTIONS =             0x00000040,
        READ_MESSAGES =             0x00000400,
        SEND_MESSAGES =             0x00000800,
        SEND_TTS_MESSAGES =         0x00001000,
        MANAGE_MESSAGES =           0x00002000,
        EMBED_LINKS =               0x00004000,
        ATTACH_FILES =              0x00008000,
        READ_MESSAGE_HISTORY =      0x00010000,
        MENTION_EVERYONE =          0x00020000,
        USE_EXTERNAL_EMOJIS =       0x00040000,

        CONNECT =                   0x00100000,
        SPEAK =                     0x00200000,
        MUTE_MEMBERS =              0x00400000,
        DEAFEN_MEMBERS =            0x00800000,
        MOVE_MEMBERS =              0x01000000,
        USE_VAD =                   0x02000000,

        MANAGE_ROLES =              0x10000000,
        MANAGE_WEBHOOKS =           0x20000000,
    }
}
