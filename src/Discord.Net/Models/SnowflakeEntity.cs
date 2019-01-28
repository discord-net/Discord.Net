using System;

namespace Discord
{
    internal abstract class SnowflakeEntity : ISnowflakeEntity
    {
        private DateTimeOffset? _createdAt;

        public SnowflakeEntity(IDiscordClient discord)
        {
            Discord = discord;
        }

        public IDiscordClient Discord { get; set; }
        public ulong Id { get; set; }

        public DateTimeOffset CreatedAt
        {
            get
            {
                if (_createdAt.HasValue)
                    return _createdAt.Value;
                _createdAt = SnowflakeUtilities.FromSnowflake(Id);
                return _createdAt.Value;
            }
        }
    }
}
