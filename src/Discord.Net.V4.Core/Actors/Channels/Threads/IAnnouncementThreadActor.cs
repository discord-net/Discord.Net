namespace Discord;

public partial interface IAnnouncementThreadChannelActor : 
    IThreadChannelActor,
    IActor<ulong, IAnnouncementThreadChannel>
{
    [LinkExtension]
    private new interface WithAnnouncementArchivedExtension : 
        IThreadChannelActor.WithAnnouncementArchivedExtension
    {
        
    }
}