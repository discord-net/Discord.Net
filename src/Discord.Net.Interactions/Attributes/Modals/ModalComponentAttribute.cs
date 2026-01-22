using System;

namespace Discord.Interactions;

/// <summary>
///     Mark an <see cref="IModal"/> property as a modal component field.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public abstract class ModalComponentAttribute : Attribute
{
    /// <summary>
    ///     Gets the type of the component.
    /// </summary>
    public abstract ComponentType ComponentType { get; }

    /// <summary>
    ///     Gets the optional identifier for component.
    /// </summary>
    /// <remarks>
    ///     Sending components with an id of 0 is allowed but will be treated as empty and replaced by the API.
    /// </remarks>
    public int Id { get; set; }

    internal ModalComponentAttribute(int id = 0)
    {
        Id = id;
    }
}
