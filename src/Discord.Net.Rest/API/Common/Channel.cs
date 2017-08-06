#pragma warning disable CS1591
using Discord.Serialization;
using System;

namespace Discord.API
{
    internal class Channel
    {
        //Shared
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("type")]
        public ChannelType Type { get; set; }
        [ModelProperty("last_message_id")]
        public ulong? LastMessageId { get; set; }

        //GuildChannel
        [ModelProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [ModelProperty("name")]
        public Optional<string> Name { get; set; }
        [ModelProperty("position")]
        public Optional<int> Position { get; set; }
        [ModelProperty("permission_overwrites")]
        public Optional<Overwrite[]> PermissionOverwrites { get; set; }

        //TextChannel
        [ModelProperty("topic")]
        public Optional<string> Topic { get; set; }
        [ModelProperty("last_pin_timestamp")]
        public Optional<DateTimeOffset?> LastPinTimestamp { get; set; }

        //VoiceChannel
        [ModelProperty("bitrate")]
        public Optional<int> Bitrate { get; set; }
        [ModelProperty("user_limit")]
        public Optional<int> UserLimit { get; set; }

        //PrivateChannel
        [ModelProperty("recipients")]
        public Optional<User[]> Recipients { get; set; }

        //GroupChannel
        [ModelProperty("icon")]
        public Optional<string> Icon { get; set; }
    }
}
