namespace Discord.Rest;

public sealed class RestGroupChannelActor :
    RestChannelActor,
    IGroupChannelActor
{
    internal RestGroupChannelActor(DiscordRestClient client, Snowflake id) : base(client, id)
    {
    }
}