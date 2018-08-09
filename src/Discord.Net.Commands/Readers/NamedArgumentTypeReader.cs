using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Commands
{
    internal sealed class NamedArgumentTypeReader<T> : TypeReader
        where T : class, new()
    {
        private static readonly TypeInfo _tInfo = typeof(T).GetTypeInfo();
        private static readonly IReadOnlyDictionary<string, PropertyInfo> _tProps = _tInfo.DeclaredProperties
            .Where(p => p.SetMethod != null && p.SetMethod.IsPublic && !p.SetMethod.IsStatic)
            .ToImmutableDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        private readonly CommandService _commands;

        public NamedArgumentTypeReader(CommandService commands)
        {
            _commands = commands;
        }

        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var result = new T();
            var state = ReadState.LookingForParameter;
            int beginRead = 0, currentRead = 0;

            while (state != ReadState.End)
            {
                var prop = Read(out var arg);
                var propVal = await ReadArgumentAsync(prop, arg).ConfigureAwait(false);
                if (propVal != null)
                    prop.SetMethod.Invoke(result, new[] { propVal });
                else
                    return TypeReaderResult.FromError(CommandError.ParseFailed, $"Could not parse the argument for the parameter '{prop.Name}' as type '{prop.PropertyType}'.");
            }

            return TypeReaderResult.FromSuccess(result);

            PropertyInfo Read(out string arg)
            {
                string currentParam = null;
                char match = '\0';

                for (; currentRead < input.Length; currentRead++)
                {
                    var currentChar = input[currentRead];
                    switch (state)
                    {
                        case ReadState.LookingForParameter:
                            if (Char.IsWhiteSpace(currentChar))
                                continue;
                            else
                            {
                                beginRead = currentRead;
                                state = ReadState.InParameter;
                            }
                            break;
                        case ReadState.InParameter:
                            if (currentChar != ':')
                                continue;
                            else
                            {
                                currentParam = input.Substring(beginRead, currentRead - beginRead);
                                state = ReadState.LookingForArgument;
                            }
                            break;
                        case ReadState.LookingForArgument:
                            if (Char.IsWhiteSpace(currentChar))
                                continue;
                            else
                            {
                                beginRead = currentRead;
                                state = (QuotationAliasUtils.GetDefaultAliasMap.TryGetValue(currentChar, out match))
                                    ? ReadState.InQuotedArgument
                                    : ReadState.InArgument;
                            }
                            break;
                        case ReadState.InArgument:
                            if (!Char.IsWhiteSpace(currentChar))
                                continue;
                            else
                                return GetPropAndValue(out arg);
                        case ReadState.InQuotedArgument:
                            if (currentChar != match)
                                continue;
                            else
                                return GetPropAndValue(out arg);
                    }
                }

                return GetPropAndValue(out arg);

                PropertyInfo GetPropAndValue(out string argv)
                {
                    state = (currentRead == input.Length)
                        ? ReadState.End
                        : ReadState.LookingForParameter;

                    var prop = _tProps[currentParam];
                    argv = input.Substring(beginRead, currentRead - beginRead);
                    return prop;
                }
            }

            async Task<object> ReadArgumentAsync(PropertyInfo prop, string arg)
            {
                var overridden = prop.GetCustomAttribute<OverrideTypeReaderAttribute>();
                var reader = (overridden != null)
                    ? ModuleClassBuilder.GetTypeReader(_commands, prop.PropertyType, overridden.TypeReader, services)
                    : (_commands.GetDefaultTypeReader(prop.PropertyType)
                        ?? _commands.GetTypeReaders(prop.PropertyType).FirstOrDefault().Value);

                if (reader != null)
                {
                    var readResult = await reader.ReadAsync(context, arg, services).ConfigureAwait(false);
                    return (readResult.IsSuccess)
                        ? readResult.BestMatch
                        : null;
                }
                return null;
            }
        }

        private enum ReadState
        {
            LookingForParameter,
            InParameter,
            LookingForArgument,
            InArgument,
            InQuotedArgument,
            End
        }
    }
}
