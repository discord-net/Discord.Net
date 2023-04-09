using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Base information class for attribute based context command handlers.
    /// </summary>
    public abstract class ContextCommandInfo : CommandInfo<CommandParameterInfo>, IApplicationCommandInfo
    {
        /// <inheritdoc/>
        public ApplicationCommandType CommandType { get; }

        /// <inheritdoc/>
        public bool DefaultPermission { get; }

        /// <inheritdoc/>
        public bool IsEnabledInDm { get; }

        /// <inheritdoc/>
        public bool IsNsfw { get; }

        /// <inheritdoc/>
        public GuildPermission? DefaultMemberPermissions { get; }

        /// <inheritdoc/>
        public override IReadOnlyList<CommandParameterInfo> Parameters { get; }

        /// <inheritdoc/>
        public override bool SupportsWildCards => false;

        /// <inheritdoc/>
        public override bool IgnoreGroupNames => true;

        internal ContextCommandInfo(Builders.ContextCommandBuilder builder, ModuleInfo module, InteractionService commandService)
            : base(builder, module, commandService)
        {
            CommandType = builder.CommandType;
            DefaultPermission = builder.DefaultPermission;
            IsNsfw = builder.IsNsfw;
            IsEnabledInDm = builder.IsEnabledInDm;
            DefaultMemberPermissions = builder.DefaultMemberPermissions;
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
        }

        internal static ContextCommandInfo Create(Builders.ContextCommandBuilder builder, ModuleInfo module, InteractionService commandService)
        {
            return builder.CommandType switch
            {
                ApplicationCommandType.User => new UserCommandInfo(builder, module, commandService),
                ApplicationCommandType.Message => new MessageCommandInfo(builder, module, commandService),
                _ => throw new InvalidOperationException("This command type is not a supported Context Command"),
            };
        }

        /// <inheritdoc/>
        protected override Task InvokeModuleEvent(IInteractionContext context, IResult result)
            => CommandService._contextCommandExecutedEvent.InvokeAsync(this, context, result);
    }
}
