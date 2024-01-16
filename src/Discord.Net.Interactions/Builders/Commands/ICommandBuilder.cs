using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represent a command builder for creating <see cref="ICommandInfo"/>.
    /// </summary>
    public interface ICommandBuilder
    {
        /// <summary>
        ///     Gets the execution delegate of this command.
        /// </summary>
        ExecuteCallback Callback { get; }

        /// <summary>
        ///     Gets the parent module of this command.
        /// </summary>
        ModuleBuilder Module { get; }

        /// <summary>
        ///     Gets the name of this command.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets or sets the method name of this command.
        /// </summary>
        string MethodName { get; set; }

        /// <summary>
        ///     Gets or sets <see langword="true"/> if this command will be registered and executed as a standalone command, unaffected by the <see cref="GroupAttribute"/>s of
        ///     of the commands parents.
        /// </summary>
        bool IgnoreGroupNames { get; set; }

        /// <summary>
        ///     Gets or sets whether the <see cref="Name"/> should be directly used as a Regex pattern.
        /// </summary>
        bool TreatNameAsRegex { get; set; }

        /// <summary>
        ///     Gets or sets the run mode this command gets executed with.
        /// </summary>
        RunMode RunMode { get; set; }

        /// <summary>
        ///     Gets a collection of the attributes of this command.
        /// </summary>
        IReadOnlyList<Attribute> Attributes { get; }

        /// <summary>
        ///     Gets a collection of the parameters of this command.
        /// </summary>
        IReadOnlyList<IParameterBuilder> Parameters { get; }

        /// <summary>
        ///     Gets a collection of the preconditions of this command.
        /// </summary>
        IReadOnlyList<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        ///     Sets <see cref="Name"/>.
        /// </summary>
        /// <param name="name">New value of the <see cref="Name"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        ICommandBuilder WithName(string name);

        /// <summary>
        ///     Sets <see cref="MethodName"/>.
        /// </summary>
        /// <param name="name">New value of the <see cref="MethodName"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        ICommandBuilder WithMethodName(string name);

        /// <summary>
        ///     Adds attributes to <see cref="Attributes"/>.
        /// </summary>
        /// <param name="attributes">New attributes to be added to <see cref="Attributes"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        ICommandBuilder WithAttributes(params Attribute[] attributes);

        /// <summary>
        ///     Sets <see cref="RunMode"/>.
        /// </summary>
        /// <param name="runMode">New value of the <see cref="RunMode"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        ICommandBuilder SetRunMode(RunMode runMode);

        /// <summary>
        ///     Sets <see cref="TreatNameAsRegex"/>.
        /// </summary>
        /// <param name="value">New value of the <see cref="TreatNameAsRegex"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        ICommandBuilder WithNameAsRegex(bool value);

        /// <summary>
        ///     Adds parameter builders to <see cref="Parameters"/>.
        /// </summary>
        /// <param name="parameters">New parameter builders to be added to <see cref="Parameters"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        ICommandBuilder AddParameters(params IParameterBuilder[] parameters);

        /// <summary>
        ///     Adds preconditions to <see cref="Preconditions"/>.
        /// </summary>
        /// <param name="preconditions">New preconditions to be added to <see cref="Preconditions"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        ICommandBuilder WithPreconditions(params PreconditionAttribute[] preconditions);
    }
}
