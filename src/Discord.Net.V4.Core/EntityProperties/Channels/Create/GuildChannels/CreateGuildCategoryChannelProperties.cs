namespace Discord;

public class CreateGuildCategoryChannelProperties : CreateGuildChannelBaseProperties
{
    protected override Optional<ChannelType> ChannelType => Discord.ChannelType.Category;
}