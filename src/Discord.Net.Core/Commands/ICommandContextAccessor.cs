namespace Discord.Commands
{
    public interface ICommandContextAccessor
    {
        ICommandContext CommandContext { get; set; }
    }
}
