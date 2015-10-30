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
	internal static class CommandParser
	{
		private enum CommandParserPart
		{
			None,
			Parameter,
			QuotedParameter,
			DoubleQuotedParameter
		}

		public static bool ParseCommand(string input, CommandMap map, out Command command, out int endPos)
		{
			int startPosition = 0;
			int endPosition = 0;
            int inputLength = input.Length;
			bool isEscaped = false;
			command = null;
			endPos = 0;

			if (input == "")
				return false;

			while (endPosition < inputLength)
			{
				char currentChar = input[endPosition++];
				if (isEscaped)
					isEscaped = false;
				else if (currentChar == '\\')
					isEscaped = true;

				if ((!isEscaped && currentChar == ' ') || endPosition >= inputLength)
				{
					int length = (currentChar == ' ' ? endPosition - 1 : endPosition) - startPosition;
					string temp = input.Substring(startPosition, length);
					if (temp == "")
						startPosition = endPosition;
					else
					{
						var newMap = map.GetMap(temp);
						if (newMap != null)
						{
							map = newMap;
							endPos = endPosition;
                        }
						else
							break;
						startPosition = endPosition;
					}
				}
			}
			command = map.GetCommand(); //Work our way backwards to find a command that matches our input
			return command != null;
		}

		public static bool ParseArgs(string input, int startPos, Command command, out CommandPart[] args)
		{
			CommandParserPart currentPart = CommandParserPart.None;
			int startPosition = startPos;
			int endPosition = startPos;
			int inputLength = input.Length;
			bool isEscaped = false;
			List<CommandPart> argList = new List<CommandPart>();
			
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

			args = argList.ToArray();
			return true;
		}
	}
}
