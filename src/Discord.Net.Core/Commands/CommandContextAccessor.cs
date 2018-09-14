using System.Threading;

namespace Discord.Commands
{
    public class CommandContextAccessor : ICommandContextAccessor
    {
        private static AsyncLocal<ICommandContext> _commandContextCurrent = new AsyncLocal<ICommandContext>();

        public ICommandContext CommandContext
        {
            get => _commandContextCurrent.Value;
            set => _commandContextCurrent.Value = value;
        }

    }
}
