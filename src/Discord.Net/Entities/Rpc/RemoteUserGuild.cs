using System;
using Model = Discord.API.Rpc.RpcUserGuild;

namespace Discord.Entities.Rpc
{
    internal class RemoteUserGuild : IRemoteUserGuild, ISnowflakeEntity
    {
        public ulong Id { get; }
        public DiscordRestClient Discord { get; }
        public string Name { get; private set; }

        public DateTimeOffset CreatedAt => DateTimeUtils.FromSnowflake(Id);

        public RemoteUserGuild(DiscordRestClient discord, Model model)
        {
            Id = model.Id;
            Discord = discord;
            Update(model, UpdateSource.Creation);
        }
        public void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest) return;
            
            Name = model.Name;
        }

        bool IEntity<ulong>.IsAttached => false;
    }
}
