using Model = Discord.API.Channel;

namespace Discord.WebSocket;

public class SocketMediaChannel : SocketForumChannel, IMediaChannel
{
    internal SocketMediaChannel(DiscordSocketClient discord, ulong id, SocketGuild guild)
        : base(discord, id, guild)
    {

    }

    internal new static SocketMediaChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        var entity = new SocketMediaChannel(guild?.Discord, model.Id, guild);
        entity.Update(state, model);
        return entity;
    }

    internal override void Update(ClientState state, Model model)
    {
        base.Update(state, model);
    }
}
