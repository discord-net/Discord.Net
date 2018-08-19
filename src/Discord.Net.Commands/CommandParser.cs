using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Commands
{
    internal static class CommandParser
    {
        public static async Task<ParseResult> ParseArgsAsync(CommandInfo command, ICommandContext context,
            bool ignoreExtraArgs, IServiceProvider services, string input, int startPos,
            IReadOnlyDictionary<char, char> aliasMap)
        {
            ParameterInfo curParam = null;
            var argBuilder = new StringBuilder(input.Length);
            var endPos = input.Length;
            var curPart = ParserPart.None;
            var lastArgEndPos = int.MinValue;
            var argList = ImmutableArray.CreateBuilder<TypeReaderResult>();
            var paramList = ImmutableArray.CreateBuilder<TypeReaderResult>();
            var isEscaping = false;
            char c, matchQuote = '\0';

            // local helper functions
            bool IsOpenQuote(IReadOnlyDictionary<char, char> dict, char ch)
            {
                // return if the key is contained in the dictionary if it is populated
                if (dict.Count != 0)
                    return dict.ContainsKey(ch);
                // or otherwise if it is the default double quote
                return c == '\"';
            }

            char GetMatch(IReadOnlyDictionary<char, char> dict, char ch)
            {
                // get the corresponding value for the key, if it exists
                // and if the dictionary is populated
                if (dict.Count != 0 && dict.TryGetValue(c, out var value))
                    return value;
                // or get the default pair of the default double quote
                return '\"';
            }

            for (var curPos = startPos; curPos <= endPos; curPos++)
            {
                c = curPos < endPos ? input[curPos] : '\0';

                //If this character is escaped, skip it
                if (isEscaping)
                    if (curPos != endPos)
                    {
                        argBuilder.Append(c);
                        isEscaping = false;
                        continue;
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
                    if (curPos == lastArgEndPos)
                        return ParseResult.FromError(CommandError.ParseFailed,
                            "There must be at least one character of whitespace between arguments.");
                    if (curParam == null)
                        curParam = command.Parameters.Count > argList.Count ? command.Parameters[argList.Count] : null;

                    if (curParam != null && curParam.IsRemainder)
                    {
                        argBuilder.Append(c);
                        continue;
                    }

                    if (IsOpenQuote(aliasMap, c))
                    {
                        curPart = ParserPart.QuotedParameter;
                        matchQuote = GetMatch(aliasMap, c);
                        continue;
                    }

                    curPart = ParserPart.Parameter;
                }

                //Has this parameter ended yet?
                string argString = null;
                switch (curPart)
                {
                    case ParserPart.Parameter when curPos == endPos || char.IsWhiteSpace(c):
                        argString = argBuilder.ToString();
                        lastArgEndPos = curPos;
                        break;
                    case ParserPart.Parameter:
                        argBuilder.Append(c);
                        break;
                    case ParserPart.QuotedParameter when c == matchQuote:
                        argString = argBuilder.ToString(); //Remove quotes
                        lastArgEndPos = curPos + 1;
                        break;
                    case ParserPart.QuotedParameter:
                        argBuilder.Append(c);
                        break;
                }

                if (argString == null) continue;
                if (curParam == null)
                {
                    if (command.IgnoreExtraArgs)
                        break;
                    return ParseResult.FromError(CommandError.BadArgCount,
                        "The input text has too many parameters.");
                }

                var typeReaderResult =
                    await curParam.ParseAsync(context, argString, services).ConfigureAwait(false);
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

            if (curParam != null && curParam.IsRemainder)
            {
                var typeReaderResult = await curParam.ParseAsync(context, argBuilder.ToString(), services)
                    .ConfigureAwait(false);
                if (!typeReaderResult.IsSuccess)
                    return ParseResult.FromError(typeReaderResult);
                argList.Add(typeReaderResult);
            }

            if (isEscaping)
                return ParseResult.FromError(CommandError.ParseFailed,
                    "Input text may not end on an incomplete escape.");
            if (curPart == ParserPart.QuotedParameter)
                return ParseResult.FromError(CommandError.ParseFailed, "A quoted parameter is incomplete");

            //Add missing optionals
            for (var i = argList.Count; i < command.Parameters.Count; i++)
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

        private enum ParserPart
        {
            None,
            Parameter,
            QuotedParameter
        }
    }
}
