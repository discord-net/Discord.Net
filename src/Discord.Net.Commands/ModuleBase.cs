using System;
using System.Threading.Tasks;
using Discord.Commands.Builders;

namespace Discord.Commands
{
    public abstract class ModuleBase : ModuleBase<ICommandContext> { }

    public abstract class ModuleBase<T> : IModuleBase
        where T : class, ICommandContext
    {
        public T Context { get; private set; }

        /// <summary>
        /// Sends a message to the source channel
        /// </summary>
        /// <param name="message">Contents of the message; optional only if <paramref name="embed"/> is specified</param>
        /// <param name="isTTS">Specifies if Discord should read this message aloud using TTS</param>
        /// <param name="embed">An embed to be displayed alongside the message</param>
        protected virtual async Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return await Context.Channel.SendMessageAsync(message, isTTS, embed, options).ConfigureAwait(false);
        }

        protected virtual void BeforeExecute(CommandInfo command)
        {
        }

        protected virtual void AfterExecute(CommandInfo command)
        {
        }

        protected virtual void OnModuleBuilding(CommandService commandService, ModuleBuilder builder)
        {
        }

        //IModuleBase
        void IModuleBase.SetContext(ICommandContext context)
        {
            var newValue = context as T;
            Context = newValue ?? throw new InvalidOperationException($"Invalid context type. Expected {typeof(T).Name}, got {context.GetType().Name}");
        }
        void IModuleBase.BeforeExecute(CommandInfo command) => BeforeExecute(command);
        void IModuleBase.AfterExecute(CommandInfo command) => AfterExecute(command);
        void IModuleBase.OnModuleBuilding(CommandService commandService, ModuleBuilder builder) => OnModuleBuilding(commandService, builder);
    }
}
