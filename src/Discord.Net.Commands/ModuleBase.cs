using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public abstract class ModuleBase : ModuleBase<ICommandContext> { }

    public abstract class ModuleBase<T> : IModuleBase
        where T : class, ICommandContext
    {
        public T Context { get; private set; }

        protected virtual async Task<IUserMessage> ReplyAsync(string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return await Context.Channel.SendMessageAsync(message, isTTS, embed, options).ConfigureAwait(false);
        }

        protected virtual void BeforeExecute(CommandInfo command)
        {
        }

        protected virtual void AfterExecute(CommandInfo command)
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
    }
}
