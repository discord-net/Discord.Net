using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Text.RegularExpressions
{
    internal static class RegexUtils
    {
        internal const byte Q = 5;    // quantifier
        internal const byte S = 4;    // ordinary stopper
        internal const byte Z = 3;    // ScanBlank stopper
        internal const byte X = 2;    // whitespace
        internal const byte E = 1;    // should be escaped

        internal static readonly byte[] _category = new byte[] {
            // 0 1 2 3 4 5 6 7 8 9 A B C D E F 0 1 2 3 4 5 6 7 8 9 A B C D E F
               0,0,0,0,0,0,0,0,0,X,X,0,X,X,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            //   ! " # $ % & ' ( ) * + , - . / 0 1 2 3 4 5 6 7 8 9 : ; < = > ?
               X,0,0,Z,S,0,0,0,S,S,Q,Q,0,0,S,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,Q,
            // @ A B C D E F G H I J K L M N O P Q R S T U V W X Y Z [ \ ] ^ _
               0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,S,S,0,S,0,
            // ' a b c d e f g h i j k l m n o p q r s t u v w x y z { | } ~
               0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,Q,S,0,0,0};

        internal static string EscapeExcluding(string input, params char[] exclude)
        {
            if (exclude is null)
                throw new ArgumentNullException("exclude");

            for (int i = 0; i < input.Length; i++)
            {
                if (IsMetachar(input[i]) && !exclude.Contains(input[i]))
                {
                    StringBuilder sb = new StringBuilder();
                    char ch = input[i];
                    int lastpos;

                    sb.Append(input, 0, i);
                    do
                    {
                        sb.Append('\\');
                        switch (ch)
                        {
                            case '\n':
                                ch = 'n';
                                break;
                            case '\r':
                                ch = 'r';
                                break;
                            case '\t':
                                ch = 't';
                                break;
                            case '\f':
                                ch = 'f';
                                break;
                        }
                        sb.Append(ch);
                        i++;
                        lastpos = i;

                        while (i < input.Length)
                        {
                            ch = input[i];
                            if (IsMetachar(ch) && !exclude.Contains(input[i]))
                                break;

                            i++;
                        }

                        sb.Append(input, lastpos, i - lastpos);

                    } while (i < input.Length);

                    return sb.ToString();
                }
            }

            return input;
        }

        internal static bool IsMetachar(char ch)
        {
            return (ch <= '|' && _category[ch] >= E);
        }

        internal static int GetWildCardCount(string input, string wildCardExpression)
        {
            var escapedWildCard = Regex.Escape(wildCardExpression);
            var match = Regex.Matches(input, $@"(?<!\\){escapedWildCard}|(?<!\\){{[0-9]+(?:,[0-9]*)?(?<!\\)}}");
            return match.Count;
        }

        internal static bool TryBuildRegexPattern<T>(T commandInfo, string wildCardStr, out string pattern) where T : class, ICommandInfo
        {
            if (commandInfo.TreatNameAsRegex)
            {
                pattern = commandInfo.Name;
                return true;
            }

            if (GetWildCardCount(commandInfo.Name, wildCardStr) == 0)
            {
                pattern = null;
                return false;
            }

            var escapedWildCard = Regex.Escape(wildCardStr);
            var unquantifiedPattern = $@"(?<!\\){escapedWildCard}(?<delimiter>[^{escapedWildCard}]?)";
            var quantifiedPattern = $@"(?<!\\){{(?<start>[0-9]+)(?<end>,[0-9]*)?(?<!\\)}}(?<delimiter>[^{escapedWildCard}]?)";

            string name = commandInfo.Name;

            var matchPairs = new SortedDictionary<int, MatchPair>();

            foreach (Match match in Regex.Matches(name, unquantifiedPattern))
                matchPairs.Add(match.Index, new(MatchType.Unquantified, match));

            foreach (Match match in Regex.Matches(name, quantifiedPattern))
                matchPairs.Add(match.Index, new(MatchType.Quantified, match));

            var sb = new StringBuilder();

            var previousMatch = 0;

            foreach (var matchPair in matchPairs)
            {
                sb.Append(Regex.Escape(name.Substring(previousMatch, matchPair.Key - previousMatch)));
                Match match = matchPair.Value.Match;
                MatchType type = matchPair.Value.Type;

                previousMatch = matchPair.Key + match.Length;

                var delimiter = match.Groups["delimiter"].Value;

                switch (type)
                {
                    case MatchType.Unquantified:
                        {
                            sb.Append(@$"([^\n\t{Regex.Escape(delimiter)}]+){Regex.Escape(delimiter)}");
                        }
                        break;
                    case MatchType.Quantified:
                        {
                            var start = match.Groups["start"].Value;
                            var end = match.Groups["end"].Value;

                            sb.Append($@"([^\n\t{Regex.Escape(delimiter)}]{{{start}{end}}}){Regex.Escape(delimiter)}");
                        }
                        break;
                }
            }

            pattern = "\\A" + sb.ToString() + "\\Z";

            return true;
        }

        private enum MatchType
        {
            Quantified,
            Unquantified
        }

        private record MatchPair
        {
            public MatchType Type { get; }
            public Match Match { get; }

            public MatchPair(MatchType type, Match match)
            {
                Type = type;
                Match = match;
            }
        }
    }
}
