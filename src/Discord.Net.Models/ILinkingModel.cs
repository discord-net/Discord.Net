using System.Diagnostics.CodeAnalysis;

namespace Discord.Models;

public interface ILinkingModel<TId, out TModel> : ILinkingModel<TId>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    bool TryGetId([MaybeNullWhen(false)] out TId id);
}

public interface ILinkingModel<TId> : ILinkingModel
    where TId : IEquatable<TId>
{
    bool TryGetId(Type modelType, [MaybeNullWhen(false)] out TId id);
}

public interface ILinkingModel
{
    bool TryGetId<TId>(Type modelType, [MaybeNullWhen(false)] out TId id)
        where TId : IEquatable<TId>
    {
        if (this is ILinkingModel<TId> link)
            return link.TryGetId(modelType, out id);

        id = default;
        return false;
    }
}