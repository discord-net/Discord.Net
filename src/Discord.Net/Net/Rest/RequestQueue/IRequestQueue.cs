using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    //TODO: Add docstrings
    public interface IRequestQueue
    {
        Task Clear(GlobalBucket type);
        Task Clear(GuildBucket type, ulong guildId);
    }
}
