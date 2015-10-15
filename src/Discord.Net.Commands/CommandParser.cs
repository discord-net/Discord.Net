using System.Collections.Generic;

namespace Discord.Commands
{
	public class CommandPart
	{
		public string Value { get; }
		public int Index { get; }

		internal CommandPart(string value, int index)
		{
			Value = value;
			Index = index;
		}
	}

	//TODO: Check support for escaping
	public static class CommandParser
	{
		private enum CommandParserPart
		{
			None,
			CommandName,
			Parameter,
			QuotedParameter,
			DoubleQuotedParameter
		}

		public static bool Parse(string input, out string command, out CommandPart[] args)
		{
			return Parse(input, out command, out args, true);
		}
		public static bool ParseArgs(string input, out CommandPart[] args)
		{
			string ignored;
			return Parse(input, out ignored, out args, false);
		}

		private static bool Parse(string input, out string command, out CommandPart[] args, bool parseCommand)
		{
			CommandParserPart currentPart = parseCommand ? CommandParserPart.CommandName : CommandParserPart.None;
			int startPosition = 0;
			int endPosition = 0;
			int inputLength = input.Length;
			bool isEscaped = false;
			List<CommandPart> argList = new List<CommandPart>();

			command = null;
			args = null;

			if (input == "")
				return false;

			while (endPosition < inputLength)
			{
				char currentChar = input[endPosition++];
				if (isEscaped)
					isEscaped = false;
				else if (currentChar == '\\')
					isEscaped = true;

				switch (currentPart)
				{
					case CommandParserPart.CommandName:
						if ((!isEscaped && currentChar == ' ') || endPosition >= inputLength)
						{
							int length = (currentChar == ' ' ? endPosition - 1 : endPosition) - startPosition;
							string temp = input.Substring(startPosition, length);
							if (temp == "")
								startPosition = endPosition;
							else
							{
								currentPart = CommandParserPart.None;
								command = temp;
								startPosition = endPosition;
							}
						}
						break;
					case CommandParserPart.None:
						if ((!isEscaped && currentChar == '\"'))
						{
							currentPart = CommandParserPart.DoubleQuotedParameter;
							startPosition = endPosition;
						}
						else if ((!isEscaped && currentChar == '\''))
						{
							currentPart = CommandParserPart.QuotedParameter;
							startPosition = endPosition;
						}
						else if ((!isEscaped && currentChar == ' ') || endPosition >= inputLength)
						{
							int length = (currentChar == ' ' ? endPosition - 1 : endPosition) - startPosition;
							string temp = input.Substring(startPosition, length);
							if (temp == "")
								startPosition = endPosition;
							else
							{
								currentPart = CommandParserPart.None;
								argList.Add(new CommandPart(temp, startPosition));
								startPosition = endPosition;
							}
						}
						break;
					case CommandParserPart.QuotedParameter:
						if ((!isEscaped && currentChar == '\''))
						{
							string temp = input.Substring(startPosition, endPosition - startPosition - 1);
							currentPart = CommandParserPart.None;
							argList.Add(new CommandPart(temp, startPosition));
							startPosition = endPosition;
						}
						else if (endPosition >= inputLength)
							return false;
						break;
					case CommandParserPart.DoubleQuotedParameter:
						if ((!isEscaped && currentChar == '\"'))
						{
							string temp = input.Substring(startPosition, endPosition - startPosition - 1);
							currentPart = CommandParserPart.None;
							argList.Add(new CommandPart(temp, startPosition));
							startPosition = endPosition;
						}
						else if (endPosition >= inputLength)
							return false;
						break;
				}
			}

			if (parseCommand && (command == null || command == ""))
				return false;

			args = argList.ToArray();
			return true;
		}
	}
}
