namespace Discord;

public class CreateGuildStageChannelProperties : CreateGuildVoiceChannelProperties
{
    protected override Optional<ChannelType> ChannelType => Discord.ChannelType.Stage;
}