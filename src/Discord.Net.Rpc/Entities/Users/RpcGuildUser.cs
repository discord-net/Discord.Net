using Model = Discord.API.Rpc.GuildMember;

namespace Discord.Rpc
{
    public class RpcGuildUser : RpcUser
    {
        private UserStatus _status;

        public override UserStatus Status => _status;
        //public object Acitivity { get; private set; }

        internal RpcGuildUser(DiscordRpcClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static RpcGuildUser Create(DiscordRpcClient discord, Model model)
        {
            var entity = new RpcGuildUser(discord, model.User.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            base.Update(model.User);
            _status = model.Status;
            //Activity = model.Activity;
        }
    }
}
