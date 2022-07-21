using Discord.Interactions.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Interactions
{
    /// <summary>
    ///     Contains the information of a Interactions Module.
    /// </summary>
    public class ModuleInfo
    {
        internal ILookup<string, PreconditionAttribute> GroupedPreconditions { get; }

        /// <summary>
        ///     Gets the underlying command service.
        /// </summary>
        public InteractionService CommandService { get; }

        /// <summary>
        ///     Gets the name of this module class.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the group name of this module, if the module is marked with a <see cref="GroupAttribute"/>.
        /// </summary>
        public string SlashGroupName { get; }

        /// <summary>
        ///     Gets <see langword="true"/> if this module is marked with a <see cref="GroupAttribute"/>.
        /// </summary>
        public bool IsSlashGroup => !string.IsNullOrEmpty(SlashGroupName);

        /// <summary>
        ///     Gets the description of this module if <see cref="IsSlashGroup"/> is <see langword="true"/>.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Gets the default Permission of this module.
        /// </summary>
        [Obsolete($"To be deprecated soon, use {nameof(IsEnabledInDm)} and {nameof(DefaultMemberPermissions)} instead.")]
        public bool DefaultPermission { get; }

        /// <summary>
        ///     Gets whether this command can be used in DMs.
        /// </summary>
        public bool IsEnabledInDm { get; }

        /// <summary>
        ///     Gets the default permissions needed for executing this command.
        /// </summary>
        public GuildPermission? DefaultMemberPermissions { get; }

        /// <summary>
        ///     Gets the collection of Sub Modules of this module.
        /// </summary>
        public IReadOnlyList<ModuleInfo> SubModules { get; }

        /// <summary>
        ///     Gets the Slash Commands that are declared in this module.
        /// </summary>
        public IReadOnlyList<SlashCommandInfo> SlashCommands { get; }

        /// <summary>
        ///     Gets the Context Commands that are declared in this module.
        /// </summary>
        public IReadOnlyList<ContextCommandInfo> ContextCommands { get; }

        /// <summary>
        ///     Gets the Component Commands that are declared in this module.
        /// </summary>
        public IReadOnlyCollection<ComponentCommandInfo> ComponentCommands { get; }

        /// <summary>
        ///     Gets the Autocomplete Commands that are declared in this module.
        /// </summary>
        public IReadOnlyCollection<AutocompleteCommandInfo> AutocompleteCommands { get; }

        public IReadOnlyCollection<ModalCommandInfo> ModalCommands { get; }

        /// <summary>
        ///     Gets the declaring type of this module, if <see cref="IsSubModule"/> is <see langword="true"/>.
        /// </summary>
        public ModuleInfo Parent { get; }

        /// <summary>
        ///     Gets <see langword="true"/> if this module is declared by another <see cref="InteractionModuleBase{T}"/>.
        /// </summary>
        public bool IsSubModule => Parent != null;

        /// <summary>
        ///     Gets a collection of the attributes of this module.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     Gets a collection of the preconditions of this module.
        /// </summary>
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        ///     Gets <see langword="true"/> if this module has a valid <see cref="GroupAttribute"/> and has no parent with a <see cref="GroupAttribute"/>.
        /// </summary>
        public bool IsTopLevelGroup { get; }

        /// <summary>
        ///     Gets <see langword="true"/> if this module will not be registered by <see cref="InteractionService.RegisterCommandsGloballyAsync(bool)"/>
        ///     or <see cref="InteractionService.RegisterCommandsToGuildAsync(ulong, bool)"/> methods.
        /// </summary>
        public bool DontAutoRegister { get; }

        internal ModuleInfo (ModuleBuilder builder, InteractionService commandService, IServiceProvider services, ModuleInfo parent = null)
        {
            CommandService = commandService;

            Name = builder.Name;
            SlashGroupName = builder.SlashGroupName;
            Description = builder.Description;
            Parent = parent;
            DefaultPermission = builder.DefaultPermission;
            IsEnabledInDm = builder.IsEnabledInDm;
            DefaultMemberPermissions = BuildDefaultMemberPermissions(builder);
            SlashCommands = BuildSlashCommands(builder).ToImmutableArray();
            ContextCommands = BuildContextCommands(builder).ToImmutableArray();
            ComponentCommands = BuildComponentCommands(builder).ToImmutableArray();
            AutocompleteCommands = BuildAutocompleteCommands(builder).ToImmutableArray();
            ModalCommands = BuildModalCommands(builder).ToImmutableArray();
            SubModules = BuildSubModules(builder, commandService, services).ToImmutableArray();
            Attributes = BuildAttributes(builder).ToImmutableArray();
            Preconditions = BuildPreconditions(builder).ToImmutableArray();
            IsTopLevelGroup = IsSlashGroup && CheckTopLevel(parent);
            DontAutoRegister = builder.DontAutoRegister;

            GroupedPreconditions = Preconditions.ToLookup(x => x.Group, x => x, StringComparer.Ordinal);
        }

        private IEnumerable<ModuleInfo> BuildSubModules (ModuleBuilder builder, InteractionService commandService, IServiceProvider services)
        {
            var result = new List<ModuleInfo>();

            foreach (Builders.ModuleBuilder moduleBuilder in builder.SubModules)
                result.Add(moduleBuilder.Build(commandService, services, this));

            return result;
        }

        private IEnumerable<SlashCommandInfo> BuildSlashCommands (ModuleBuilder builder)
        {
            var result = new List<SlashCommandInfo>();

            foreach (Builders.SlashCommandBuilder commandBuilder in builder.SlashCommands)
                result.Add(commandBuilder.Build(this, CommandService));

            return result;
        }

        private IEnumerable<ContextCommandInfo> BuildContextCommands (ModuleBuilder builder)
        {
            var result = new List<ContextCommandInfo>();

            foreach (Builders.ContextCommandBuilder commandBuilder in builder.ContextCommands)
                result.Add(commandBuilder.Build(this, CommandService));

            return result;
        }

        private IEnumerable<ComponentCommandInfo> BuildComponentCommands (ModuleBuilder builder)
        {
            var result = new List<ComponentCommandInfo>();

            foreach (var interactionBuilder in builder.ComponentCommands)
                result.Add(interactionBuilder.Build(this, CommandService));

            return result;
        }

        private IEnumerable<AutocompleteCommandInfo> BuildAutocompleteCommands( ModuleBuilder builder)
        {
            var result = new List<AutocompleteCommandInfo>();

            foreach (var commandBuilder in builder.AutocompleteCommands)
                result.Add(commandBuilder.Build(this, CommandService));

            return result;
        }

        private IEnumerable<ModalCommandInfo> BuildModalCommands(ModuleBuilder builder)
        {
            var result = new List<ModalCommandInfo>();

            foreach (var commandBuilder in builder.ModalCommands)
                result.Add(commandBuilder.Build(this, CommandService));

            return result;
        }

        private IEnumerable<Attribute> BuildAttributes (ModuleBuilder builder)
        {
            var result = new List<Attribute>();
            var currentParent = builder;

            while (currentParent != null)
            {
                result.AddRange(currentParent.Attributes);
                currentParent = currentParent.Parent;
            }

            return result;
        }

        private static IEnumerable<PreconditionAttribute> BuildPreconditions (ModuleBuilder builder)
        {
            var preconditions = new List<PreconditionAttribute>();

            var parent = builder;

            while (parent != null)
            {
                preconditions.AddRange(parent.Preconditions);
                parent = parent.Parent;
            }

            return preconditions;
        }

        private static bool CheckTopLevel (ModuleInfo parent)
        {
            var currentParent = parent;

            while (currentParent != null)
            {
                if (currentParent.IsSlashGroup)
                    return false;

                currentParent = currentParent.Parent;
            }
            return true;
        }

        private static GuildPermission? BuildDefaultMemberPermissions(ModuleBuilder builder)
        {
            var permissions = builder.DefaultMemberPermissions;

            var parent = builder.Parent;

            while (parent != null)
            {
                permissions = (permissions ?? 0) | (parent.DefaultMemberPermissions ?? 0).SanitizeGuildPermissions();
                parent = parent.Parent;
            }

            return permissions;
        }
    }
}
