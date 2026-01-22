using System;
using System.Collections.Generic;
using System.Reflection;

namespace Discord.Interactions.Builders;

public abstract class ModalComponentBuilder<TInfo, TBuilder> : IModalComponentBuilder
    where TInfo : ModalComponentInfo
    where TBuilder : ModalComponentBuilder<TInfo, TBuilder>
{
    private readonly List<Attribute> _attributes;
    protected abstract TBuilder Instance { get; }

    /// <inheritdoc/>
    public ModalBuilder Modal { get; }

    /// <inheritdoc/>
    public ComponentType ComponentType { get; internal set; }

    /// <inheritdoc/>
    public Type Type { get; private set; }

    /// <inheritdoc/>
    public PropertyInfo PropertyInfo { get; internal set; }

    /// <inheritdoc/>
    public object DefaultValue { get; set; }

    /// <inheritdoc/>
    public int Id { get; set; }

    /// <inheritdoc/>
    public IReadOnlyCollection<Attribute> Attributes => _attributes;

    internal ModalComponentBuilder(ModalBuilder modal)
    {
        Modal = modal;
        _attributes = new();
    }

    /// <summary>
    ///     Sets <see cref="ComponentType"/>.
    /// </summary>
    /// <param name="componentType">New value of the <see cref="ComponentType"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public virtual TBuilder WithComponentType(ComponentType componentType)
    {
        ComponentType = componentType;
        return Instance;
    }

    /// <summary>
    ///     Sets <see cref="Type"/>.
    /// </summary>
    /// <param name="type">New value of the <see cref="Type"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public virtual TBuilder WithType(Type type)
    {
        Type = type;
        return Instance;
    }

    /// <summary>
    ///     Sets <see cref="DefaultValue"/>.
    /// </summary>
    /// <param name="value">New value of the <see cref="DefaultValue"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public virtual TBuilder SetDefaultValue(object value)
    {
        DefaultValue = value;
        return Instance;
    }

    /// <summary>
    ///     Adds attributes to <see cref="Attributes"/>.
    /// </summary>
    /// <param name="attributes">New attributes to be added to <see cref="Attributes"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public virtual TBuilder WithAttributes(params Attribute[] attributes)
    {
        _attributes.AddRange(attributes);
        return Instance;
    }

    /// <summary>
    ///     Sets <see cref="Id"/>.
    /// </summary>
    /// <param name="id">New value of the <see cref="Id"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public virtual TBuilder WithId(int id)
    {
        Id = id;
        return Instance;
    }

    internal abstract TInfo Build(ModalInfo modal);

    /// <inheritdoc/>
    IModalComponentBuilder IModalComponentBuilder.WithType(Type type) => WithType(type);

    /// <inheritdoc/>
    IModalComponentBuilder IModalComponentBuilder.SetDefaultValue(object value) => SetDefaultValue(value);

    /// <inheritdoc/>
    IModalComponentBuilder IModalComponentBuilder.WithAttributes(params Attribute[] attributes) => WithAttributes(attributes);

    /// <inheritdoc/>
    IModalComponentBuilder IModalComponentBuilder.WithId(int id) => WithId(id);
}
