using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Provides a base class for a command module to inherit from.
    /// </summary>
    /// <typeparam name="T">Type of interaction context to be injected into the module.</typeparam>
    public abstract class InteractionModuleBase<T> : IInteractionModuleBase where T : class, IInteractionContext
    {
        /// <summary>
        ///     Gets the underlying context of the command.
        /// </summary>
        public T Context { get; private set; }

        /// <inheritdoc/>
        public virtual void AfterExecute (ICommandInfo command) { }

        /// <inheritdoc/>
        public virtual void BeforeExecute (ICommandInfo command) { }

        /// <inheritdoc/>
        public virtual Task BeforeExecuteAsync(ICommandInfo command) => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task AfterExecuteAsync(ICommandInfo command) => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual void OnModuleBuilding (InteractionService commandService, ModuleInfo module) { }

        /// <inheritdoc/>
        public virtual void Construct (Builders.ModuleBuilder builder, InteractionService commandService) { }

        internal void SetContext (IInteractionContext context)
        {
            var newValue = context as T;
            Context = newValue ?? throw new InvalidOperationException($"Invalid context type. Expected {typeof(T).Name}, got {context.GetType().Name}.");
        }

        /// <inheritdoc cref="IDiscordInteraction.DeferAsync(bool, RequestOptions)"/>
        protected virtual async Task DeferAsync(bool ephemeral = false, RequestOptions options = null) =>
            await Context.Interaction.DeferAsync(ephemeral, options).ConfigureAwait(false);

        /// <inheritdoc cref="IDiscordInteraction.RespondAsync(string, Embed[], bool, bool, AllowedMentions, RequestOptions, MessageComponent, Embed)"/>
        protected virtual async Task RespondAsync (string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null, Embed embed = null) =>
            await Context.Interaction.RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);

        /// <inheritdoc cref="IDiscordInteraction.FollowupAsync(string, Embed[], bool, bool, AllowedMentions, RequestOptions, MessageComponent, Embed)"/>
        protected virtual async Task<IUserMessage> FollowupAsync (string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null, Embed embed = null) =>
            await Context.Interaction.FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);

        /// <inheritdoc cref="IMessageChannel.SendMessageAsync(string, bool, Embed, RequestOptions, AllowedMentions, MessageReference, MessageComponent, ISticker[], Embed[])"/>
        protected virtual async Task<IUserMessage> ReplyAsync (string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null,
            AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent components = null) =>
            await Context.Channel.SendMessageAsync(text, false, embed, options, allowedMentions, messageReference, components).ConfigureAwait(false);

        /// <inheritdoc cref="IDeletable.DeleteAsync(RequestOptions)"/>
        protected virtual async Task DeleteOriginalResponseAsync ( )
        {
            var response = await Context.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
            await response.DeleteAsync().ConfigureAwait(false);
        }

        //IInteractionModuleBase

        /// <inheritdoc/>
        void IInteractionModuleBase.SetContext (IInteractionContext context) => SetContext(context);
    }

    /// <summary>
    ///     Provides a base class for a command module to inherit from.
    /// </summary>
    public abstract class InteractionModuleBase : InteractionModuleBase<IInteractionContext> { }
}
