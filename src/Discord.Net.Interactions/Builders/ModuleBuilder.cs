using System;
using System.Collections.Generic;
using System.Reflection;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="ModuleInfo"/>.
    /// </summary>
    public class ModuleBuilder
    {
        private readonly List<Attribute> _attributes;
        private readonly List<PreconditionAttribute> _preconditions;
        private readonly List<ModuleBuilder> _subModules;
        private readonly List<SlashCommandBuilder> _slashCommands;
        private readonly List<ContextCommandBuilder> _contextCommands;
        private readonly List<ComponentCommandBuilder> _componentCommands;
        private readonly List<AutocompleteCommandBuilder> _autocompleteCommands;

        /// <summary>
        ///     Gets the underlying Interaction Service.
        /// </summary>
        public InteractionService InteractionService { get; }

        /// <summary>
        ///     Gets the parent module if this module is a sub-module.
        /// </summary>
        public ModuleBuilder Parent { get; }

        /// <summary>
        ///     Gets the name of this module.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        ///     Gets and sets the group name of this module.
        /// </summary>
        public string SlashGroupName { get; set; }

        /// <summary>
        ///     Gets whether this has a <see cref="GroupAttribute"/>.
        /// </summary>
        public bool IsSlashGroup => !string.IsNullOrEmpty(SlashGroupName);

        /// <summary>
        ///     Gets and sets the description of this module.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets and sets the default permission of this module.
        /// </summary>
        public bool DefaultPermission { get; set; } = true;

        /// <summary>
        ///     Gets and sets whether this has a <see cref="DontAutoRegisterAttribute"/>.
        /// </summary>
        public bool DontAutoRegister { get; set; } = false;

        /// <summary>
        ///     Gets a collection of the attributes of this module.
        /// </summary>
        public IReadOnlyList<Attribute> Attributes => _attributes;

        /// <summary>
        ///     Gets a collection of the preconditions of this module.
        /// </summary>
        public IReadOnlyCollection<PreconditionAttribute> Preconditions => _preconditions;

        /// <summary>
        ///     Gets a collection of the sub-modules of this module.
        /// </summary>
        public IReadOnlyList<ModuleBuilder> SubModules => _subModules;

        /// <summary>
        ///     Gets a collection of the Slash Commands of this module.
        /// </summary>
        public IReadOnlyList<SlashCommandBuilder> SlashCommands => _slashCommands;

        /// <summary>
        ///     Gets a collection of the Context Commands of this module.
        /// </summary>
        public IReadOnlyList<ContextCommandBuilder> ContextCommands => _contextCommands;

        /// <summary>
        ///     Gets a collection of the Component Commands of this module.
        /// </summary>
        public IReadOnlyList<ComponentCommandBuilder> ComponentCommands => _componentCommands;

        /// <summary>
        ///     Gets a collection of the Autocomplete Commands of this module.
        /// </summary>
        public IReadOnlyList<AutocompleteCommandBuilder> AutocompleteCommands => _autocompleteCommands;

        internal TypeInfo TypeInfo { get; set; }

        internal ModuleBuilder (InteractionService interactionService, ModuleBuilder parent = null)
        {
            InteractionService = interactionService;
            Parent = parent;

            _attributes = new List<Attribute>();
            _subModules = new List<ModuleBuilder>();
            _slashCommands = new List<SlashCommandBuilder>();
            _contextCommands = new List<ContextCommandBuilder>();
            _componentCommands = new List<ComponentCommandBuilder>();
            _autocompleteCommands = new List<AutocompleteCommandBuilder>();
            _preconditions = new List<PreconditionAttribute>();
        }

        /// <summary>
        ///     Initializes a new <see cref="ModuleBuilder"/>.
        /// </summary>
        /// <param name="interactionService">The underlying Interaction Service.</param>
        /// <param name="name">Name of this module.</param>
        /// <param name="parent">Parent module of this sub-module.</param>
        public ModuleBuilder (InteractionService interactionService, string name, ModuleBuilder parent = null) : this(interactionService, parent)
        {
            Name = name;
        }

        /// <summary>
        ///     Sets <see cref="SlashGroupName"/>.
        /// </summary>
        /// <param name="name">New value of the <see cref="SlashGroupName"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModuleBuilder WithGroupName (string name)
        {
            SlashGroupName = name;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="Description"/>.
        /// </summary>
        /// <param name="description">New value of the <see cref="Description"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModuleBuilder WithDescription (string description)
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
        public ModuleBuilder WithDefaultPermision (bool permission)
        {
            DefaultPermission = permission;
            return this;
        }

        /// <summary>
        ///     Adds attributes to <see cref="Attributes"/>
        /// </summary>
        /// <param name="attributes">New attributes to be added to <see cref="Attributes"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModuleBuilder AddAttributes (params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return this;
        }

        /// <summary>
        ///     Adds preconditions to <see cref="Preconditions"/>
        /// </summary>
        /// <param name="preconditions">New preconditions to be added to <see cref="Preconditions"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModuleBuilder AddPreconditions (params PreconditionAttribute[] preconditions)
        {
            _preconditions.AddRange(preconditions);
            return this;
        }

        /// <summary>
        ///     Adds slash command builder to <see cref="SlashCommands"/>
        /// </summary>
        /// <param name="configure"><see cref="SlashCommandBuilder"/> factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModuleBuilder AddSlashCommand (Action<SlashCommandBuilder> configure)
        {
            var command = new SlashCommandBuilder(this);
            configure(command);
            _slashCommands.Add(command);
            return this;
        }

        /// <summary>
        ///     Adds context command builder to <see cref="ContextCommands"/>
        /// </summary>
        /// <param name="configure"><see cref="ContextCommandBuilder"/> factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModuleBuilder AddContextCommand (Action<ContextCommandBuilder> configure)
        {
            var command = new ContextCommandBuilder(this);
            configure(command);
            _contextCommands.Add(command);
            return this;
        }

        /// <summary>
        ///     Adds component command builder to <see cref="ComponentCommands"/>
        /// </summary>
        /// <param name="configure"><see cref="ComponentCommandBuilder"/> factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModuleBuilder AddComponentCommand (Action<ComponentCommandBuilder> configure)
        {
            var command = new ComponentCommandBuilder(this);
            configure(command);
            _componentCommands.Add(command);
            return this;
        }

        /// <summary>
        ///     Adds autocomplete command builder to <see cref="AutocompleteCommands"/>
        /// </summary>
        /// <param name="configure"><see cref="AutocompleteCommands"/> factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModuleBuilder AddAutocompleteCommand(Action<AutocompleteCommandBuilder> configure)
        {
            var command = new AutocompleteCommandBuilder(this);
            configure(command);
            _autocompleteCommands.Add(command);
            return this;
        }

        /// <summary>
        ///     Adds sub-module builder to <see cref="SubModules"/>
        /// </summary>
        /// <param name="configure"><see cref="ModuleBuilder"/> factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModuleBuilder AddModule (Action<ModuleBuilder> configure)
        {
            var subModule = new ModuleBuilder(InteractionService, this);
            configure(subModule);
            _subModules.Add(subModule);
            return this;
        }

        internal ModuleInfo Build (InteractionService interactionService, IServiceProvider services, ModuleInfo parent = null)
        {
            var moduleInfo = new ModuleInfo(this, interactionService, services, parent);

            IInteractionModuleBase instance = ReflectionUtils<IInteractionModuleBase>.CreateObject(TypeInfo, interactionService, services);
            try
            {
                instance.OnModuleBuilding(interactionService, moduleInfo);
            }
            finally
            {
                ( instance as IDisposable )?.Dispose();
            }

            return moduleInfo;
        }
    }
}
