using Model = Discord.API.Channel;

namespace Discord.Rest;

public class RestMediaChannel : RestForumChannel, IMediaChannel
{
    internal RestMediaChannel(BaseDiscordClient client, IGuild guild, ulong id)
        : base(client, guild, id)
    {

    }

    internal new static RestMediaChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
    {
        var entity = new RestMediaChannel(discord, guild, model.Id);
        entity.Update(model);
        return entity;
    }

    internal override void Update(Model model)
    {
        base.Update(model);
    }
}
