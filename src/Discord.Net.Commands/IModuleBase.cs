namespace Discord.Commands
{
    internal interface IModuleBase
    {
        void SetContext(ICommandContext context);

        void BeforeExecute(CommandInfo command);
        
        void AfterExecute(CommandInfo command);

        void OnModuleAdded(CommandService commandService);
    }
}
