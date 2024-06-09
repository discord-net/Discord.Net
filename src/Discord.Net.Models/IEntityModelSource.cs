namespace Discord.Models;

/// <summary>
///     Represents a type that contains other entities within itself.
/// </summary>
public interface IEntityModelSource
{
    /// <summary>
    ///     Gets a collection of other entities defined on this type.
    /// </summary>
    /// <returns>A collection of other entities defined on this type.</returns>
    IEnumerable<IEntityModel> GetEntities();
}
