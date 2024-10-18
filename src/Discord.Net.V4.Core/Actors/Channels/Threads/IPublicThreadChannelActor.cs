namespace Discord;

public partial interface IPublicThreadChannelActor :
    IThreadChannelActor,
    IActor<ulong, IPublicThreadChannel>
{
    
}