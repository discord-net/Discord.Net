using System;
using Discord.Rest;
using Model = Discord.API.GuildMember;
using PresenceModel = Discord.API.Presence;

namespace Discord.WebSocket
{
    internal class SocketGuildUser : GuildUser, ISocketUser, IVoiceState
    {
        internal override bool IsAttached => true;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new SocketGuild Guild => base.Guild as SocketGuild;
        public new SocketGlobalUser User => base.User as SocketGlobalUser;
        public Presence Presence => User.Presence; //{ get; private set; }

        public override Game Game => Presence.Game;
        public override UserStatus Status => Presence.Status;

        public VoiceState? VoiceState
        {
            get
            {
                return Guild.GetVoiceState(Id);
            }
        }

        public bool IsSelfDeafened
        {
            get
            {
                return VoiceState?.IsSelfDeafened ?? false;
            }
        }
        public bool IsSelfMuted
        {
            get
            {
                return VoiceState?.IsSelfMuted ?? false;
            }
        }
        public bool IsSuppressed
        {
            get
            {
                return VoiceState?.IsSuppressed ?? false;
            }
        }
        public VoiceChannel VoiceChannel
        {
            get
            {
                return VoiceState?.VoiceChannel;
            }
        }

        public bool IsDeafened
        {
            get
            {
                return VoiceState?.IsDeafened ?? false;
            }
        }

        public bool IsMuted
        {
            get
            {
                return VoiceState?.IsMuted ?? false;
            }
        }

        IVoiceChannel IVoiceState.VoiceChannel
        {
            get
            {
                return VoiceState?.VoiceChannel;
            }
        }

        public string VoiceSessionId
        {
            get
            {
                return VoiceState?.VoiceSessionId ?? "";
            }
        }

        public SocketGuildUser(SocketGuild guild, SocketGlobalUser user, Model model) 
            : base(guild, user, model)
        {
            //Presence = new Presence(null, UserStatus.Offline);
        }
        public SocketGuildUser(SocketGuild guild, SocketGlobalUser user, PresenceModel model)
            : base(guild, user, model)
        {
        }

        public override void Update(PresenceModel model, UpdateSource source)
        {
            base.Update(model, source);

            var game = model.Game != null ? new Game(model.Game) : null;
            //Presence = new Presence(game, model.Status);

            User.Update(model, source);
        }

        public SocketGuildUser Clone() => MemberwiseClone() as SocketGuildUser;
        ISocketUser ISocketUser.Clone() => Clone();
    }
}
