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
        internal override SocketGlobalUser GlobalUser { get => this; set => throw new NotImplementedException(); }

        private readonly object _lockObj = new object();
        private ushort _references;

        private SocketGlobalUser(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {

        }
        internal static SocketGlobalUser Create(DiscordSocketClient discord, ClientStateManager state, Model model)
        {
            var entity = new SocketGlobalUser(discord, model.Id);
            entity.Update(state, model);
            return entity;
        }

        internal void AddRef()
        {
            checked
            {
                lock (_lockObj)
                    _references++;
            }
        }
        internal void RemoveRef(DiscordSocketClient discord)
        {
            lock (_lockObj)
            {
                if (--_references <= 0)
                    discord.RemoveUser(Id);
            }
        }

        ~SocketGlobalUser() => Discord.StateManager.RemoveReferencedGlobalUser(Id);
        public void Dispose() => Discord.StateManager.RemoveReferencedGlobalUser(Id);

        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")}, Global)";
        internal new SocketGlobalUser Clone() => MemberwiseClone() as SocketGlobalUser;
    }
}
