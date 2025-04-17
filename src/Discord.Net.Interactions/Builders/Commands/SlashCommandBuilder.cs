using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="SlashCommandInfo"/>.
    /// </summary>
    public sealed class SlashCommandBuilder : CommandBuilder<SlashCommandInfo, SlashCommandBuilder, SlashCommandParameterBuilder>
    {
        protected override SlashCommandBuilder Instance => this;

        /// <summary>
        ///     Gets and sets the description of this command.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets and sets the default permission of this command.
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
        ///     Gets or sets the install method for this command.
        /// </summary>
        public HashSet<ApplicationIntegrationType> IntegrationTypes { get; set; } = null;

        /// <summary>
        ///     Gets or sets the context types this command can be executed in.
        /// </summary>
        public HashSet<InteractionContextType> ContextTypes { get; set; } = null;

        internal SlashCommandBuilder(ModuleBuilder module) : base(module)
        {
            IntegrationTypes = module.IntegrationTypes;
            ContextTypes = module.ContextTypes;
#pragma warning disable CS0618 // Type or member is obsolete
            IsEnabledInDm = module.IsEnabledInDm;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        ///     Initializes a new <see cref="SlashCommandBuilder"/>.
        /// </summary>
        /// <param name="module">Parent module of this command.</param>
        /// <param name="name">Name of this command.</param>
        /// <param name="callback">Execution callback of this command.</param>
        public SlashCommandBuilder(ModuleBuilder module, string name, ExecuteCallback callback) : base(module, name, callback) { }

        /// <summary>
        ///     Sets <see cref="Description"/>.
        /// </summary>
        /// <param name="description">New value of the <see cref="Description"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="DefaultPermission"/>.
        /// </summary>
        /// <param name="permission">New value of the <see cref="DefaultPermission"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        [Obsolete($"To be deprecated soon, use {nameof(SetEnabledInDm)} and {nameof(WithDefaultMemberPermissions)} instead.")]
        public SlashCommandBuilder WithDefaultPermission(bool permission)
        {
            DefaultPermission = permission;
            return Instance;
        }

        /// <summary>
        ///     Adds a command parameter to the parameters collection.
        /// </summary>
        /// <param name="configure"><see cref="SlashCommandParameterBuilder"/> factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public override SlashCommandBuilder AddParameter(Action<SlashCommandParameterBuilder> configure)
        {
            var parameter = new SlashCommandParameterBuilder(this);
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
        public SlashCommandBuilder SetEnabledInDm(bool isEnabled)
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
        public SlashCommandBuilder SetNsfw(bool isNsfw)
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
        public SlashCommandBuilder WithDefaultMemberPermissions(GuildPermission permissions)
        {
            DefaultMemberPermissions = permissions;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="IntegrationTypes"/> on this <see cref="SlashCommandBuilder"/>.
        /// </summary>
        /// <param name="integrationTypes">Install types for this command.</param>
        /// <returns>The builder instance.</returns>
        public SlashCommandBuilder WithIntegrationTypes(params ApplicationIntegrationType[] integrationTypes)
        {
            IntegrationTypes = integrationTypes is not null
                ? new HashSet<ApplicationIntegrationType>(integrationTypes)
                : null;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="ContextTypes"/> on this <see cref="SlashCommandBuilder"/>.
        /// </summary>
        /// <param name="contextTypes">Context types the command can be executed in.</param>
        /// <returns>The builder instance.</returns>
        public SlashCommandBuilder WithContextTypes(params InteractionContextType[] contextTypes)
        {
            ContextTypes = contextTypes is not null
                ? new HashSet<InteractionContextType>(contextTypes)
                : null;
            return this;
        }

        internal override SlashCommandInfo Build(ModuleInfo module, InteractionService commandService) =>
            new SlashCommandInfo(this, module, commandService);
    }
}
