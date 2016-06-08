using Model = Discord.API.Channel;

namespace Discord
{
    internal interface ICachedChannel : IChannel, ICachedEntity<ulong>
    {
        void Update(Model model, UpdateSource source);

        ICachedChannel Clone();
    }
}
