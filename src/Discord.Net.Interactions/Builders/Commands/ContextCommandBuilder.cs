using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="ContextCommandInfo"/>.
    /// </summary>
    public sealed class ContextCommandBuilder : CommandBuilder<ContextCommandInfo, ContextCommandBuilder, CommandParameterBuilder>
    {
        protected override ContextCommandBuilder Instance => this;

        /// <summary>
        ///     Gets the type of this command.
        /// </summary>
        public ApplicationCommandType CommandType { get; set; }

        /// <summary>
        ///     Gets the default permission of this command.
        /// </summary>
        [Obsolete($"To be deprecated soon, use {nameof(IsEnabledInDm)} and {nameof(DefaultMemberPermissions)} instead.")]
        public bool DefaultPermission { get; set; } = true;

        /// <summary>
        ///     Gets whether this command can be used in DMs.
        /// </summary>
        public bool IsEnabledInDm { get; set; } = true;

        /// <summary>
        ///     Gets whether this command is age restricted.
        /// </summary>
        public bool IsNsfw { get; set; } = false;

        /// <summary>
        ///     Gets the default permissions needed for executing this command.
        /// </summary>
        public GuildPermission? DefaultMemberPermissions { get; set; } = null;

        /// <summary>
        ///     Gets the install method for this command.
        /// </summary>
        public HashSet<ApplicationIntegrationType> IntegrationTypes { get; set; } = null;

        /// <summary>
        ///     Gets the context types this command can be executed in.
        /// </summary>
        public HashSet<InteractionContextType> ContextTypes { get; set; } = null;

        internal ContextCommandBuilder(ModuleBuilder module) : base(module)
        {
            IntegrationTypes = module.IntegrationTypes;
            ContextTypes = module.ContextTypes;
#pragma warning disable CS0618 // Type or member is obsolete
            IsEnabledInDm = module.IsEnabledInDm;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        ///     Initializes a new <see cref="ContextCommandBuilder"/>.
        /// </summary>
        /// <param name="module">Parent module of this command.</param>
        /// <param name="name">Name of this command.</param>
        /// <param name="callback">Execution callback of this command.</param>
        public ContextCommandBuilder(ModuleBuilder module, string name, ExecuteCallback callback) : base(module, name, callback) { }

        /// <summary>
        ///     Sets <see cref="CommandType"/>.
        /// </summary>
        /// <param name="commandType">New value of the <see cref="CommandType"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ContextCommandBuilder SetType(ApplicationCommandType commandType)
        {
            CommandType = commandType;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="DefaultPermission"/>.
        /// </summary>
        /// <param name="defaultPermision">New value of the <see cref="DefaultPermission"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        [Obsolete($"To be deprecated soon, use {nameof(SetEnabledInDm)} and {nameof(WithDefaultMemberPermissions)} instead.")]
        public ContextCommandBuilder SetDefaultPermission(bool defaultPermision)
        {
            DefaultPermission = defaultPermision;
            return this;
        }

        /// <summary>
        ///     Adds a command parameter to the parameters collection.
        /// </summary>
        /// <param name="configure"><see cref="CommandParameterBuilder"/> factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public override ContextCommandBuilder AddParameter(Action<CommandParameterBuilder> configure)
        {
            var parameter = new CommandParameterBuilder(this);
            configure(parameter);
            AddParameters(parameter);
            return this;
        }

        /// <summary>
        ///     Sets <see cref="IsEnabledInDm"/>.
        /// </summary>
        /// <param name="isEnabled">New value of the <see cref="IsEnabledInDm"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ContextCommandBuilder SetEnabledInDm(bool isEnabled)
        {
            IsEnabledInDm = isEnabled;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="IsNsfw"/>.
        /// </summary>
        /// <param name="isNsfw">New value of the <see cref="IsNsfw"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ContextCommandBuilder SetNsfw(bool isNsfw)
        {
            IsNsfw = isNsfw;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="DefaultMemberPermissions"/>.
        /// </summary>
        /// <param name="permissions">New value of the <see cref="DefaultMemberPermissions"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ContextCommandBuilder WithDefaultMemberPermissions(GuildPermission permissions)
        {
            DefaultMemberPermissions = permissions;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="IntegrationTypes"/> of this <see cref="ContextCommandBuilder"/>.
        /// </summary>
        /// <param name="integrationTypes">Install types for this command.</param>
        /// <returns>The builder instance.</returns>
        public ContextCommandBuilder WithIntegrationTypes(params ApplicationIntegrationType[] integrationTypes)
        {
            IntegrationTypes = new HashSet<ApplicationIntegrationType>(integrationTypes);
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="ContextTypes"/> of this <see cref="ContextCommandBuilder"/>.
        /// </summary>
        /// <param name="contextTypes">Context types the command can be executed in.</param>
        /// <returns>The builder instance.</returns>
        public ContextCommandBuilder WithContextTypes(params InteractionContextType[] contextTypes)
        {
            ContextTypes = new HashSet<InteractionContextType>(contextTypes);
            return this;
        }

        internal override ContextCommandInfo Build(ModuleInfo module, InteractionService commandService) =>
            ContextCommandInfo.Create(this, module, commandService);
    }
}
