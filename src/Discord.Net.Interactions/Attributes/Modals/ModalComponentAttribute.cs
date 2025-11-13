using System;

namespace Discord.Interactions.Attributes.Modals;

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

    internal ModalComponentAttribute() { }
}
