using System;

namespace Discord.Commands
{
	public class CommandServiceConfig
    {
        /// <summary> Gets or sets the prefix character used to trigger commands, if ActivationMode has the Char flag set. </summary>
		public char? PrefixChar { get { return _prefixChar; } set { SetValue(ref _prefixChar, value); } }
		private char? _prefixChar = null;

        /// <summary> Gets or sets whether a message beginning with a mention to the logged-in user should be treated as a command. </summary>
        public bool AllowMentionPrefix { get { return _allowMentionPrefix; } set { SetValue(ref _allowMentionPrefix, value); } }
        private bool _allowMentionPrefix = true;

        /// <summary> 
        /// Gets or sets a custom function used to detect messages that should be treated as commands.
        /// This function should a positive one indicating the index of where the in the message's RawText the command begins, 
        /// and a negative value if the message should be ignored.
        /// </summary>
        public Func<Message, int> CustomPrefixHandler { get { return _customPrefixHandler; } set { SetValue(ref _customPrefixHandler, value); } }
        private Func<Message, int> _customPrefixHandler = null;

        /// <summary> Gets or sets whether a help function should be automatically generated. </summary>
		public HelpMode HelpMode { get { return _helpMode; } set { SetValue(ref _helpMode, value); } }
		private HelpMode _helpMode = HelpMode.Disable;

        //Lock
        protected bool _isLocked;
		internal void Lock() { _isLocked = true; }
		protected void SetValue<T>(ref T storage, T value)
		{
			if (_isLocked)
				throw new InvalidOperationException("Unable to modify a service's configuration after it has been created.");
			storage = value;
		}
    }
}
