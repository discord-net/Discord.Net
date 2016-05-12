using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    public interface IRequestQueue
    {
        Task Clear(GlobalBucket type);
        Task Clear(GuildBucket type, ulong guildId);
    }
}
