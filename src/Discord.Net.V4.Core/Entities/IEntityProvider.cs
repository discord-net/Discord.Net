using Discord.Models;

namespace Discord;

public interface IEntityProvider<T, U, V> : IClientProvider
    where T : class, U, IConstructable<V>
    where V : IEntityModel?
{

}

