namespace Discord.Rest;

public class RestGuildMessageActor : 
    RestMessageActor,
    IGuildMessageActor
{
    internal RestGuildMessageActor(DiscordRestClient client, IRestMessageChannelTrait channel, Snowflake id) : base(client, channel, id)
    {
    }
}