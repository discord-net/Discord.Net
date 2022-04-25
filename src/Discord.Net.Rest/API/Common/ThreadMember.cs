using Newtonsoft.Json;
using System;

namespace Discord.API
{
    internal class ThreadMember : IThreadMemberModel
    {
        [JsonProperty("id")]
        public Optional<ulong> ThreadId { get; set; }

        [JsonProperty("user_id")]
        public Optional<ulong> UserId { get; set; }

        [JsonProperty("join_timestamp")]
        public DateTimeOffset JoinTimestamp { get; set; }

        ulong? IThreadMemberModel.ThreadId { get => ThreadId.ToNullable(); set => throw new NotSupportedException(); }
        DateTimeOffset IThreadMemberModel.JoinedAt { get => JoinTimestamp; set => throw new NotSupportedException(); }
        ulong IEntityModel<ulong>.Id { get => UserId.GetValueOrDefault(0); set => throw new NotSupportedException(); }
    }
}
