using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating <see cref="ModalInfo"/>.
/// </summary>
public class ModalBuilder
{
    internal readonly InteractionService _interactionService;
    internal readonly List<IInputComponentBuilder> _components;

    /// <summary>
    ///     Gets the initialization delegate for this modal.
    /// </summary>
    public ModalInitializer ModalInitializer { get; internal set; }

    /// <summary>
    ///     Gets the title of this modal.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Gets the <see cref="IModal"/> implementation used to initialize this object.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    ///     Gets a collection of the components of this modal.
    /// </summary>
    public IReadOnlyCollection<IInputComponentBuilder> Components => _components;

    internal ModalBuilder(Type type, InteractionService interactionService)
    {
        if (!typeof(IModal).IsAssignableFrom(type))
            throw new ArgumentException($"Must be an implementation of {nameof(IModal)}", nameof(type));

        Type = type;

        _interactionService = interactionService;
        _components = new();
    }

    /// <summary>
    ///     Initializes a new <see cref="ModalBuilder"/>
    /// </summary>
    /// <param name="modalInitializer">The initialization delegate for this modal.</param>
    public ModalBuilder(Type type, ModalInitializer modalInitializer, InteractionService interactionService) : this(type, interactionService)
    {
        ModalInitializer = modalInitializer;
    }

    /// <summary>
    ///     Sets <see cref="Title"/>.
    /// </summary>
    /// <param name="title">New value of the <see cref="Title"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ModalBuilder WithTitle(string title)
    {
        Title = title;
        return this;
    }

    /// <summary>
    ///     Adds text components to <see cref="Components"/>.
    /// </summary>
    /// <param name="configure">Text Component builder factory.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ModalBuilder AddTextComponent(Action<TextInputComponentBuilder> configure)
    {
        var builder = new TextInputComponentBuilder(this);
        configure(builder);
        _components.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a select menu component to <see cref="Components"/>.
    /// </summary>
    /// <param name="configure">Select menu component builder factory.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ModalBuilder AddSelectMenuComponent(Action<SelectMenuInputComponentBuilder> configure)
    {
        var builder = new SelectMenuInputComponentBuilder(this);
        configure(builder);
        _components.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a user select component to <see cref="Components"/>.
    /// </summary>
    /// <param name="configure">User select component builder factory.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ModalBuilder AddUserSelectComponent(Action<UserSelectInputComponentBuilder> configure)
    {
        var builder = new UserSelectInputComponentBuilder(this);
        configure(builder);
        _components.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a role select component to <see cref="Components"/>.
    /// </summary>
    /// <param name="configure">Role select component builder factory.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ModalBuilder AddRoleSelectComponent(Action<RoleSelectInputComponentBuilder> configure)
    {
        var builder = new RoleSelectInputComponentBuilder(this);
        configure(builder);
        _components.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a mentionable select component to <see cref="Components"/>.
    /// </summary>
    /// <param name="configure">Mentionable select component builder factory.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ModalBuilder AddMentionableSelectComponent(Action<MentionableSelectInputComponentBuilder> configure)
    {
        var builder = new MentionableSelectInputComponentBuilder(this);
        configure(builder);
        _components.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a channel select component to <see cref="Components"/>.
    /// </summary>
    /// <param name="configure">Channel select component builder factory.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ModalBuilder AddChannelSelectComponent(Action<ChannelSelectInputComponentBuilder> configure)
    {
        var builder = new ChannelSelectInputComponentBuilder(this);
        configure(builder);
        _components.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a file upload component to <see cref="Components"/>.
    /// </summary>
    /// <param name="configure">File upload component builder factory.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ModalBuilder AddFileUploadComponent(Action<FileUploadInputComponentBuilder> configure)
    {
        var builder = new FileUploadInputComponentBuilder(this);
        configure(builder);
        _components.Add(builder);
        return this;
    }

    internal ModalInfo Build() => new(this);
}
