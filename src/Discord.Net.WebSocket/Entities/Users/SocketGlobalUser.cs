using System;
using System.Diagnostics;
using System.Linq;
using Model = Discord.IUserModel;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal class SocketGlobalUser : SocketUser, IDisposable
    {
        public override bool IsBot { get; internal set; }
        public override string Username { get; internal set; }
        public override ushort DiscriminatorValue { get; internal set; }
        public override string AvatarId { get; internal set; }
        public override bool IsWebhook => false;

        private SocketGlobalUser(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {

        }
        internal static SocketGlobalUser Create(DiscordSocketClient discord, Model model)
        {
            var entity = new SocketGlobalUser(discord, model.Id);
            entity.Update(model);
            return entity;
        }

        ~SocketGlobalUser() => Dispose();
        public override void Dispose()
        {
            GC.SuppressFinalize(this);
            Discord.StateManager.UserStore.RemoveReference(Id);
        }

        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")}, Global)";
        internal new SocketGlobalUser Clone() => MemberwiseClone() as SocketGlobalUser;
    }
}
