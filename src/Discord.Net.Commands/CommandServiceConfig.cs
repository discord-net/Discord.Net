using System;

namespace Discord.Commands
{
	public enum HelpMode
	{
		/// <summary> Disable the automatic help command. </summary>
		Disable,
		/// <summary> Use the automatic help command and respond in the channel the command is used. </summary>
		Public,
		/// <summary> Use the automatic help command and respond in a private message. </summary>
		Private
	}
    [Flags]
    public enum ActivationMode
    {
        // All of these probably need to be changed
        /// <summary> Enable command activation by char. </summary>
        Char = 0x1,
        /// <summary> Enable command activation when mentioned. </summary>
        Mention = 0x2,
        /// <summary> Enable command activation by custom function. </summary>
        Custom = 0x4
    }
	public class CommandServiceConfig
	{
		public char? CommandChar
		{
			get
			{
				return _commandChars.Length > 0 ? _commandChars[0] : (char?)null;
			}
			set
			{
				if (value != null)
					CommandChars = new char[] { value.Value };
				else
					CommandChars = new char[0];
			}
		}
		public char[] CommandChars { get { return _commandChars; } set { SetValue(ref _commandChars, value); } }
		private char[] _commandChars = new char[] { '!' };
        
        public Func<Message, int> CustomActivator { get { return _customActivator; } set { SetValue(ref _customActivator, value); } }
        private Func<Message, int> _customActivator = null;

		public HelpMode HelpMode { get { return _helpMode; } set { SetValue(ref _helpMode, value); } }
		private HelpMode _helpMode = HelpMode.Disable;

        public ActivationMode ActivationMode { get { return _activationMode; } set { SetValue(ref _activationMode, value); } }
        private ActivationMode _activationMode = ActivationMode.Char; // Set char as default, not sure if it's the best method of doing it

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
