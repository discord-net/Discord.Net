using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
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

        private static readonly MethodInfo _convertArrayMethod = typeof(CommandParser).GetTypeInfo().GetDeclaredMethod(nameof(ConvertParamsList));
        private static readonly ConcurrentDictionary<Type, Func<List<object>, object>> _arrayConverters = new ConcurrentDictionary<Type, Func<List<object>, object>>();
        
        public static async Task<ParseResult> ParseArgs(Command command, IMessage context, string input, int startPos)
        {
            CommandParameter curParam = null;
            StringBuilder argBuilder = new StringBuilder(input.Length);
            int endPos = input.Length;
            var curPart = ParserPart.None;
            int lastArgEndPos = int.MinValue;
            var argList = ImmutableArray.CreateBuilder<object>();
            List<object> paramsList = null; // TODO: could we use a better type?
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
                    if (!typeReaderResult.IsSuccess)
                        return ParseResult.FromError(typeReaderResult);

                    if (curParam.IsMultiple)
                    {
                        if (paramsList == null)
                            paramsList = new List<object>();
                        paramsList.Add(typeReaderResult.Value);

                        if (curPos == endPos)
                        {
                            var func = _arrayConverters.GetOrAdd(curParam.ElementType, t =>
                            {
                                var method = _convertArrayMethod.MakeGenericMethod(t);
                                return (Func<List<object>, object>)method.CreateDelegate(typeof(Func<List<object>, object>));
                            });
                            argList.Add(func.Invoke(paramsList));

                            curParam = null;
                            curPart = ParserPart.None;
                        }
                    }
                    else
                    {
                        argList.Add(typeReaderResult.Value);

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
                argList.Add(typeReaderResult.Value);
            }

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

        private static T[] ConvertParamsList<T>(List<object> paramsList)
        {
            var array = new T[paramsList.Count];
            for (int i = 0; i < array.Length; i++)
                array[i] = (T)paramsList[i];
            return array;
        }
    }
}
