namespace Discord.Commands
{
    internal interface IModuleBase
    {
        void SetContext(ICommandContext context);

        void BeforeExecute(OverloadInfo overload);
        
        void AfterExecute(OverloadInfo overload);
    }
}
