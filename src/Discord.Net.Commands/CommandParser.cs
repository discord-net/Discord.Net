using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Commands
{
    internal static class CommandParser
    {
        private enum ParserPart
        {
            None,
            Parameter,
            QuotedParameter
        }
        
        public static async Task<ParseResult> ParseArgs(Command command, IMessage context, string input, int startPos)
        {
            CommandParameter curParam = null;
            StringBuilder argBuilder = new StringBuilder(input.Length);
            int endPos = input.Length;
            var curPart = ParserPart.None;
            int lastArgEndPos = int.MinValue;
            var argList = ImmutableArray.CreateBuilder<object>();
            bool isEscaping = false;
            char c;

            for (int curPos = startPos; curPos <= endPos; curPos++)
            {
                if (curPos < endPos)
                    c = input[curPos];
                else
                    c = '\0';

                //If this character is escaped, skip it
                if (isEscaping)
                {
                    if (curPos != endPos)
                    {
                        argBuilder.Append(c);
                        isEscaping = false;
                        continue;
                    }
                }
                //Are we escaping the next character?
                if (c == '\\' && (curParam == null || !curParam.IsUnparsed))
                {
                    isEscaping = true;
                    continue;
                }

                //If we're processing an unparsed parameter, ignore all other logic
                if (curParam != null && curParam.IsUnparsed && curPos != endPos)
                {
                    argBuilder.Append(c);
                    continue;
                }

                //If we're not currently processing one, are we starting the next argument yet?
                if (curPart == ParserPart.None)
                {
                    if (char.IsWhiteSpace(c) || curPos == endPos)
                        continue; //Skip whitespace between arguments
                    else if (curPos == lastArgEndPos)
                        return ParseResult.FromError(CommandError.ParseFailed, "There must be at least one character of whitespace between arguments.");
                    else
                    {
                        curParam = command.Parameters.Count > argList.Count ? command.Parameters[argList.Count] : null;
                        if (curParam != null && curParam.IsUnparsed)
                        {
                            argBuilder.Append(c);
                            continue;
                        }
                        if (c == '\"')
                        {
                            curPart = ParserPart.QuotedParameter;
                            continue;
                        }
                        curPart = ParserPart.Parameter;
                    }
                }

                //Has this parameter ended yet?
                string argString = null;
                if (curPart == ParserPart.Parameter)
                {
                    if (curPos == endPos || char.IsWhiteSpace(c))
                    {
                        argString = argBuilder.ToString();
                        lastArgEndPos = curPos;
                    }
                    else
                        argBuilder.Append(c);
                }
                else if (curPart == ParserPart.QuotedParameter)
                {
                    if (c == '\"')
                    {
                        argString = argBuilder.ToString(); //Remove quotes
                        lastArgEndPos = curPos + 1;
                    }
                    else
                        argBuilder.Append(c);
                }
                                
                if (argString != null)
                {
                    if (curParam == null)
                        return ParseResult.FromError(CommandError.BadArgCount, "The input text has too many parameters.");

                    var typeReaderResult = await curParam.Parse(context, argString).ConfigureAwait(false);
                    if (!typeReaderResult.IsSuccess)
                        return ParseResult.FromError(typeReaderResult);
                    argList.Add(typeReaderResult.Value);

                    curParam = null;
                    curPart = ParserPart.None;
                    argBuilder.Clear();
                }
            }

            if (curParam != null && curParam.IsUnparsed)
                argList.Add(argBuilder.ToString());

            if (isEscaping)
                return ParseResult.FromError(CommandError.ParseFailed, "Input text may not end on an incomplete escape.");
            if (curPart == ParserPart.QuotedParameter)
                return ParseResult.FromError(CommandError.ParseFailed, "A quoted parameter is incomplete");

            if (argList.Count < command.Parameters.Count)
            {
                for (int i = argList.Count; i < command.Parameters.Count; i++)
                {
                    var param = command.Parameters[i];
                    if (!param.IsOptional)
                        return ParseResult.FromError(CommandError.BadArgCount, "The input text has too few parameters.");
                    argList.Add(param.DefaultValue);
                }
            }

            return ParseResult.FromSuccess(argList.ToImmutable());
        }
    }
}
