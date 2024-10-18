namespace Discord;

public partial interface IPrivateThreadChannelActor :
    IThreadChannelActor,
    IActor<ulong, IPrivateThreadChannel>
{
    
}