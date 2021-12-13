[SlashCommand("name", "Description")]
public async Task Command([ChannelTypes(ChannelType.Stage, ChannelType.Text)] IChannel channel)
{
    ...
}
