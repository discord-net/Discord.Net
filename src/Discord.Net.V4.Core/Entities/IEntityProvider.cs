using Discord.Models;

namespace Discord;

public interface IEntityProvider<T, U, V> : IClientSource
    where T : class, U, IConstructable<V>
    where V : IEntityModel?
{
    
}

