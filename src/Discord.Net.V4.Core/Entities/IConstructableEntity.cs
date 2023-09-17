using Discord.Models;
using System.Runtime.Versioning;

namespace Discord;

[RequiresPreviewFeatures]
public interface IConstructableEntity<TModel>
    where TModel : IEntityModel
{
    internal static abstract T Construct<T>(TModel model);
    
    static T Create<T>(TModel model)
        where T : IConstructableEntity<TModel>
    {
        return T.Construct<T>(model);
    }
}
