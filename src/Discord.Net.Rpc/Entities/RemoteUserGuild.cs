using Discord.Rest;
using System;
using Model = Discord.API.Rpc.RpcUserGuild;

namespace Discord.Rpc
{
    /*internal class RemoteUserGuild : IRemoteUserGuild, ISnowflakeEntity
    {
        public ulong Id { get; }
        public DiscordRestClient Discord { get; }
        public string Name { get; private set; }

        public DateTimeOffset CreatedAt => DateTimeUtils.FromSnowflake(Id);

        public RemoteUserGuild(DiscordRestClient discord, Model model)
        {
            Id = model.Id;
            Discord = discord;
            Update(model);
        }
        public void Update(Model model)
        {
            Name = model.Name;
        }

        bool IEntity<ulong>.IsAttached => false;
    }*/
}
