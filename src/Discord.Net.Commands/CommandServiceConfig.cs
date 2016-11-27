namespace Discord.Commands
{
    public class CommandServiceConfig
    {
        /// <summary> The default RunMode commands should have, if one is not specified on the Command attribute or builder. </summary>
        public RunMode DefaultRunMode { get; set; } = RunMode.Mixed;
        /// <summary> What character should be used between command nodes</summary>
        public string NodeSeparator { get; set; } = " ";
    }
}
