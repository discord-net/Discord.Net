namespace Discord.Commands
{
    public interface IModuleBase<out T>
        where T : ICommandContext
    {
    }

    internal interface IModuleBase
    {
        void SetContext(ICommandContext context);

        void BeforeExecute();
        
        void AfterExecute();
    }
}
