namespace Discord.Commands
{
    internal interface IModuleBase
    {
        void SetContext(ICommandContext context);

        void BeforeExecute();
        
        void AfterExecute();
    }
}
