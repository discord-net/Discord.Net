using Discord.Models;
using System.Runtime.Versioning;

namespace Discord;

public interface IConstructable<TModel>
    where TModel : IEntityModel?
{
    internal static abstract T Construct<T>(TModel model);
    
    static T Create<T>(TModel model)
        where T : IConstructable<TModel>
    {
        return T.Construct<T>(model);
    }
}
