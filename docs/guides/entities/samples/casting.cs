// Say we have an entity, for the simplicity of this example, it will appear from thin air.
IChannel channel;

// If I want this to be an ITextChannel so I can access the properties of a text channel inside of a guild, an approach would be:
ITextChannel textChannel = channel as ITextChannel;

await textChannel.DoSomethingICantWithIChannelAsync();
