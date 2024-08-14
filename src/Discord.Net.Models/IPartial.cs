using System.Diagnostics.CodeAnalysis;

namespace Discord.Models;

public interface IPartial<in T> : IPartial
{
    bool ApplyTo(T model);
}

public interface IPartial
{
    Type UnderlyingModelType { get; }
    
    bool TryGet<U>(string property, [MaybeNullWhen(false)] out U value);

    bool Has(string property);
}