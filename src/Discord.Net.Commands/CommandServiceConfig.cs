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
	public class CommandServiceConfig
	{
		public Func<User, int> PermissionResolver { get { return _permissionsResolver; } set { SetValue(ref _permissionsResolver, value); } }
        private Func<User, int> _permissionsResolver;

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

		public HelpMode HelpMode { get { return _helpMode; } set { SetValue(ref _helpMode, value); } }
		private HelpMode _helpMode = HelpMode.Disable;

		//Lock
		protected bool _isLocked;
		internal void Lock() { _isLocked = true; }
		protected void SetValue<T>(ref T storage, T value)
		{
			if (_isLocked)
				throw new InvalidOperationException("Unable to modify a discord client's configuration after it has been created.");
			storage = value;
		}
	}
}
