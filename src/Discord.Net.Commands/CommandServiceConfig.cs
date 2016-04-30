using System;

namespace Discord.Commands
{
    public class CommandServiceConfigBuilder
    {
        /// <summary> Gets or sets the prefix character used to trigger commands, if ActivationMode has the Char flag set. </summary>
		public char? PrefixChar { get; set; } = null;
        /// <summary> Gets or sets whether a message beginning with a mention to the logged-in user should be treated as a command. </summary>
        public bool AllowMentionPrefix { get; set; } = true;
        /// <summary> 
        /// Gets or sets a custom function used to detect messages that should be treated as commands.
        /// This function should a positive one indicating the index of where the in the message's RawText the command begins, 
        /// and a negative value if the message should be ignored.
        /// </summary>
        public Func<Message, int> CustomPrefixHandler { get; set; } = null;
        /// <summary>
        /// Changing this to true makes the bot ignore all messages, except when the messages are from its own account.
        /// This is desired behavior for "Self Bots" only, so unless this bot is being run under a normal user's account, leave it alone!!
        /// </summary>
        public bool IsSelfBot { get; set; } = false;

        /// <summary> Gets or sets whether a help function should be automatically generated. </summary>
		public HelpMode HelpMode { get; set; } = HelpMode.Disabled;


        /// <summary> Gets or sets a handler that is called on any successful command execution. </summary>
        public EventHandler<CommandEventArgs> ExecuteHandler { get; set; }
        /// <summary> Gets or sets a handler that is called on any error during command parsing or execution. </summary>
        public EventHandler<CommandErrorEventArgs> ErrorHandler { get; set; }

        public CommandServiceConfig Build() => new CommandServiceConfig(this);
    }
    public class CommandServiceConfig
    {
        public char? PrefixChar { get; }
        public bool AllowMentionPrefix { get; }
        public Func<Message, int> CustomPrefixHandler { get; }
        public bool IsSelfBot { get; }

        /// <summary> Gets or sets whether a help function should be automatically generated. </summary>
		public HelpMode HelpMode { get; set; } = HelpMode.Disabled;

        internal CommandServiceConfig(CommandServiceConfigBuilder builder)
        {
            PrefixChar = builder.PrefixChar;
            AllowMentionPrefix = builder.AllowMentionPrefix;
            CustomPrefixHandler = builder.CustomPrefixHandler;
            HelpMode = builder.HelpMode;
            IsSelfBot = builder.IsSelfBot;
        }
    }
}
