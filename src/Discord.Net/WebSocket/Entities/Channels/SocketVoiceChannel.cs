using Discord.Audio;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    internal class SocketVoiceChannel : VoiceChannel, ISocketGuildChannel
    {
        internal override bool IsAttached => true;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new SocketGuild Guild => base.Guild as SocketGuild;

        public IReadOnlyCollection<IGuildUser> Members 
            => Guild.VoiceStates.Where(x => x.Value.VoiceChannel.Id == Id).Select(x => Guild.GetUser(x.Key)).ToImmutableArray();

        public SocketVoiceChannel(SocketGuild guild, Model model)
            : base(guild, model)
        {
        }

        public override Task<IGuildUser> GetUserAsync(ulong id) 
            => Task.FromResult(GetUser(id));
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync() 
            => Task.FromResult(Members);
        public IGuildUser GetUser(ulong id)
        {
            var user = Guild.GetUser(id);
            if (user != null && user.VoiceChannel.Id == Id)
                return user;
            return null;
        }

        public override async Task<IAudioClient> ConnectAsync()
        {
            var audioMode = Discord.AudioMode;
            if (audioMode == AudioMode.Disabled)
                throw new InvalidOperationException($"Audio is not enabled on this client, {nameof(DiscordSocketConfig.AudioMode)} in {nameof(DiscordSocketConfig)} must be set.");
                        
            return await Guild.ConnectAudioAsync(Id,
                (audioMode & AudioMode.Incoming) == 0,
                (audioMode & AudioMode.Outgoing) == 0).ConfigureAwait(false);
        }

        public SocketVoiceChannel Clone() => MemberwiseClone() as SocketVoiceChannel;

        ISocketChannel ISocketChannel.Clone() => Clone();
    }
}
