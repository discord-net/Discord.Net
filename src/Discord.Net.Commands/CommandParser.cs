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
        
        public static async Task<ParseResult> ParseArgs(CommandInfo command, ICommandContext context, string input, int startPos)
        {
            ParameterInfo curParam = null;
            StringBuilder argBuilder = new StringBuilder(input.Length);
            int endPos = input.Length;
            var curPart = ParserPart.None;
            int lastArgEndPos = int.MinValue;
            var argList = ImmutableArray.CreateBuilder<TypeReaderResult>();
            var paramList = ImmutableArray.CreateBuilder<TypeReaderResult>();
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
                if (c == '\\' && (curParam == null || !curParam.IsRemainder))
                {
                    isEscaping = true;
                    continue;
                }

                //If we're processing an remainder parameter, ignore all other logic
                if (curParam != null && curParam.IsRemainder && curPos != endPos)
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
                        if (curParam == null)
                            curParam = command.Parameters.Count > argList.Count ? command.Parameters[argList.Count] : null;

                        if (curParam != null && curParam.IsRemainder)
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
                    if (!typeReaderResult.IsSuccess && typeReaderResult.Error != CommandError.MultipleMatches)
                        return ParseResult.FromError(typeReaderResult);

                    if (curParam.IsMultiple)
                    {
                        paramList.Add(typeReaderResult);

                        curPart = ParserPart.None;
                    }
                    else
                    {
                        argList.Add(typeReaderResult);

                        curParam = null;
                        curPart = ParserPart.None;
                    }
                    argBuilder.Clear();
                }
            }

            if (curParam != null && curParam.IsRemainder)
            {
                var typeReaderResult = await curParam.Parse(context, argBuilder.ToString()).ConfigureAwait(false);
                if (!typeReaderResult.IsSuccess)
                    return ParseResult.FromError(typeReaderResult);
                argList.Add(typeReaderResult);
            }

            if (isEscaping)
                return ParseResult.FromError(CommandError.ParseFailed, "Input text may not end on an incomplete escape.");
            if (curPart == ParserPart.QuotedParameter)
                return ParseResult.FromError(CommandError.ParseFailed, "A quoted parameter is incomplete");
            
            //Add missing optionals
            for (int i = argList.Count; i < command.Parameters.Count; i++)
            {
                var param = command.Parameters[i];
                if (param.IsMultiple)
                    continue;
                if (!param.IsOptional)
                    return ParseResult.FromError(CommandError.BadArgCount, "The input text has too few parameters.");
                argList.Add(TypeReaderResult.FromSuccess(param.DefaultValue));
            }
            
            return ParseResult.FromSuccess(argList.ToImmutable(), paramList.ToImmutable());
        }
    }
}
