using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the base info class for <see cref="IModal"/> input components.
    /// </summary>
    public abstract class InputComponentInfo
    {
        /// <summary>
        ///     Gets the parent modal of this component.
        /// </summary>
        public ModalInfo Modal { get; }

        /// <summary>
        ///     Gets the custom id of this component.
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     Gets the label of this component.
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///     Gets whether or not this component requires a user input.
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        ///     Gets the type of this component.
        /// </summary>
        public ComponentType ComponentType { get; }

        /// <summary>
        ///     Gets the reference type of this component.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     Gets the default value of this component.
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        ///     Gets a collection of the attributes of this command.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        protected InputComponentInfo(Builders.IInputComponentBuilder builder, ModalInfo modal)
        {
            Modal = modal;
            CustomId = builder.CustomId;
            Label = builder.Label;
            IsRequired = builder.IsRequired;
            ComponentType = builder.ComponentType;
            Type = builder.Type;
            DefaultValue = builder.DefaultValue;
            Attributes = builder.Attributes.ToImmutableArray();
        }
    }
}
