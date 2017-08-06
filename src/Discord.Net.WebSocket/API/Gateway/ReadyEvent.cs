#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class ReadyEvent
    {
        public class ReadState
        {
            [ModelProperty("id")]
            public string ChannelId { get; set; }
            [ModelProperty("mention_count")]
            public int MentionCount { get; set; }
            [ModelProperty("last_message_id")]
            public string LastMessageId { get; set; }
        }

        [ModelProperty("v")]
        public int Version { get; set; }
        [ModelProperty("user")]
        public User User { get; set; }
        [ModelProperty("session_id")]
        public string SessionId { get; set; }
        [ModelProperty("read_state")]
        public ReadState[] ReadStates { get; set; }
        [ModelProperty("guilds")]
        public ExtendedGuild[] Guilds { get; set; }
        [ModelProperty("private_channels")]
        public Channel[] PrivateChannels { get; set; }
        [ModelProperty("relationships")]
        public Relationship[] Relationships { get; set; }

        //Ignored
        /*[ModelProperty("user_settings")]
        [ModelProperty("user_guild_settings")]
        [ModelProperty("tutorial")]*/
    }
}
