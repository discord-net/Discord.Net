using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace Discord.Interactions;

/// <summary>
///     Represents the base info class for <see cref="IModal"/> components.
/// </summary>
public abstract class ModalComponentInfo
{
    private Lazy<Func<object, object>> _getter;
    internal Func<object, object> Getter => _getter.Value;


    /// <summary>
    ///     Gets the parent modal of this component.
    /// </summary>
    public ModalInfo Modal { get; }

    /// <summary>
    ///     Gets the type of this component.
    /// </summary>
    public ComponentType ComponentType { get; }

    /// <summary>
    ///     Gets the reference type of this component.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    ///     Gets the property linked to this component.
    /// </summary>
    public PropertyInfo PropertyInfo { get; }

    /// <summary>
    ///     Gets the default value of this component property.
    /// </summary>
    public object DefaultValue { get; }

    /// <summary>
    ///     Gets a collection of the attributes of this command.
    /// </summary>
    public IReadOnlyCollection<Attribute> Attributes { get; }

    internal ModalComponentInfo(Builders.IModalComponentBuilder builder, ModalInfo modal)
    {
        Modal = modal;
        ComponentType = builder.ComponentType;
        Type = builder.Type;
        PropertyInfo = builder.PropertyInfo;
        DefaultValue = builder.DefaultValue;
        Attributes = builder.Attributes.ToImmutableArray();

        _getter = new(() => ReflectionUtils<object>.CreateLambdaPropertyGetter(Modal.Type, PropertyInfo));
    }
}
