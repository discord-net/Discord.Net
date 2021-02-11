using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    public class SlashCommandInfo
    {
        /// <summary>
        ///     Gets the module that the command belongs in.
        /// </summary>
        public SlashModuleInfo Module { get; }
        /// <summary>
        ///     Gets the name of the command.
        /// </summary>
        public string Name { get; }
        /// <summary>
        ///     Gets the name of the command.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The user method as a delegate. We need to use Delegate because there is an unknown number of parameters
        /// </summary>
        public Delegate userMethod;
        /// <summary>
        /// The callback that we call to start the delegate.
        /// </summary>
        public Func<object[], Task<IResult>> callback;

        public SlashCommandInfo(SlashModuleInfo module, string name, string description, Delegate userMethod)
        {
            Module = module;
            Name = name;
            Description = description;
            this.userMethod = userMethod;
            this.callback = new Func<object[], Task<IResult>>(async (args) =>
            {
                // Try-catch it and see what we get - error or success
                try
                {
                    await Task.Run(() =>
                    {
                        userMethod.DynamicInvoke(args);
                    }).ConfigureAwait(false);
                }
                catch(Exception e)
                {
                    return ExecuteResult.FromError(e);
                }
                return ExecuteResult.FromSuccess();

            });
        }

        public async Task<IResult> ExecuteAsync(object[] args)
        {
            return await callback.Invoke(args).ConfigureAwait(false);
        }
    }
}
