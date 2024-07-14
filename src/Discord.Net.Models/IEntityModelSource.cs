namespace Discord.Models;

/// <summary>
///     Represents a type that contains other entities within itself.
/// </summary>
public interface IModelSource
{
    /// <summary>
    ///     Gets a collection of other models defined on this type.
    /// </summary>
    /// <returns>A collection of other entities defined on this type.</returns>
    IEnumerable<IEntityModel> GetDefinedModels();
}

public interface IModelSourceOf<out TModel>
{
    TModel Model { get; }
}

public interface IModelSourceOfMultiple<out TModel> : IModelSource
    where TModel : IEntityModel
{
    IEnumerable<TModel> GetModels();

    IEnumerable<IEntityModel> IModelSource.GetDefinedModels() => (IEnumerable<IEntityModel>) GetModels();
}
