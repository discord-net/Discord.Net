using Model = Discord.API.Rpc.GuildStatusEvent;

namespace Discord.Rpc
{
    public class RpcGuildStatus
    {
        public RpcGuild Guild { get; }
        public int Online { get; private set; }

        internal RpcGuildStatus(ulong guildId)
        {
            Guild = new RpcGuild(guildId);
        }
        internal static RpcGuildStatus Create(Model model)
        {
            var entity = new RpcGuildStatus(model.Guild.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            Online = model.Online;
        }
    }
}
