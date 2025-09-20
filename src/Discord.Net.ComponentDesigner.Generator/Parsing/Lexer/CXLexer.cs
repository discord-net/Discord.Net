using Microsoft.CodeAnalysis.Text;
using System;
using System.Threading;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed class CXLexer
{
    private ref struct TokenInfo
    {
        public int Start;
        public int End;

        public CXTokenKind Kind;
        public CXTokenFlags Flags;

        public int LeadingTriviaLength;
        public int TrailingTriviaLength;
    }

    public enum LexMode
    {
        Default,
        StringLiteral,
        Identifier,
        ElementValue,
        Attribute
    }

    public const string COMMENT_START = "<!--";
    public const string COMMENT_END = "-->";

    public const char NULL_CHAR = '\0';
    public const char NEWLINE_CHAR = '\n';
    public const char CARRAGE_RETURN_CHAR = '\r';

    public const char UNDERSCORE_CHAR = '_';
    public const char HYPHEN_CHAR = '-';
    public const char PERIOD_CHAR = '.';

    public const char LESS_THAN_CHAR = '<';
    public const char GREATER_THAN_CHAR = '>';
    public const char FORWARD_SLASH_CHAR = '/';
    public const char BACK_SLASH_CHAR = '\\';

    public const char EQUALS_CHAR = '=';
    public const char QUOTE_CHAR = '\'';
    public const char DOUBLE_QUOTE_CHAR = '"';

    public CXSourceReader Reader { get; }

    public int? InterpolationIndex { get; private set; }

    public TextSpan? CurrentInterpolationSpan
    {
        get
        {
            // there's no next interpolation
            if (Reader.Source.Interpolations.Length <= _interpolationIndex) return null;


            for (; _interpolationIndex < Reader.Source.Interpolations.Length; _interpolationIndex++)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var interpolationSpan = Reader.Source.Interpolations[_interpolationIndex];

                if (interpolationSpan.End < Reader.Position) continue;

                // either we're in the interpolation or it's ahead of us
                if (interpolationSpan.Contains(Reader.Position)) return interpolationSpan;

                // it's ahead of us
                break;
            }

            return null;
        }
    }

    public TextSpan? NextInterpolationSpan
    {
        get
        {
            // there's no next interpolation
            if (Reader.Source.Interpolations.Length <= _nextInterpolationIndex) return null;

            // check if it's ahead of us
            var interpolationSpan = Reader.Source.Interpolations[_nextInterpolationIndex];

            if (interpolationSpan.End > Reader.Position) return interpolationSpan;

            for (; _nextInterpolationIndex < Reader.Source.Interpolations.Length; _nextInterpolationIndex++)
            {
                CancellationToken.ThrowIfCancellationRequested();

                interpolationSpan = Reader.Source.Interpolations[_nextInterpolationIndex];
                if (interpolationSpan.Start > Reader.Position) break;
            }

            return interpolationSpan;
        }
    }

    private int InterpolationBoundary
        => CurrentInterpolationSpan?.Start ??
           NextInterpolationSpan?.Start ??
           Reader.Source.SourceSpan.End;

    public bool ForcedEscapedQuotes => Reader.Source.WrappingQuoteCount == 1;

    public LexMode Mode { get; set; }

    public CXToken[] InterpolationMap;

    public char? QuoteChar;

    private int _nextInterpolationIndex;
    private int _interpolationIndex;

    public CancellationToken CancellationToken { get; set; }


    public CXLexer(
        CXSourceReader reader,
        CancellationToken cancellationToken = default
    )
    {
        CancellationToken = cancellationToken;
        Reader = reader;
        Mode = LexMode.Default;
        InterpolationMap = new CXToken[Reader.Source.Interpolations.Length];
    }

    public void Seek(int position)
    {
        Reader.Position = position;
        _interpolationIndex = 0;
        _nextInterpolationIndex = 0;
    }

    public void Reset()
    {
        InterpolationMap = new CXToken[Reader.Source.Interpolations.Length];
    }

    public readonly struct ModeSentinel(CXLexer? lexer) : IDisposable
    {
        private readonly LexMode _mode = lexer?.Mode ?? LexMode.Default;

        public void Dispose()
        {
            if (lexer is null) return;

            lexer.Mode = _mode;
        }
    }

    public ModeSentinel SetMode(LexMode mode)
    {
        if (mode == Mode) return default;

        var sentinel = new ModeSentinel(this);
        Mode = mode;
        return sentinel;
    }

    public CXToken Next()
    {
        InterpolationIndex = null;

        var info = default(TokenInfo);

        info.Start = Reader.Position;

        GetTrivia(isTrailing: false, ref info.LeadingTriviaLength);

        Scan(ref info);

        GetTrivia(isTrailing: true, ref info.TrailingTriviaLength);

        info.End = Reader.Position;

        var fullSpan = TextSpan.FromBounds(info.Start, info.End);

        var token = new CXToken(
            info.Kind,
            fullSpan,
            info.LeadingTriviaLength,
            info.TrailingTriviaLength,
            info.Flags,
            fullSpan.IsEmpty ? string.Empty : Reader.Source.GetValue(fullSpan)
        );

        if (info.Kind is CXTokenKind.Interpolation && InterpolationIndex.HasValue)
            InterpolationMap[InterpolationIndex.Value] = token;

        return token;
    }

    private void Scan(ref TokenInfo info)
    {
        switch (Mode)
        {
            case LexMode.StringLiteral:
                LexStringLiteral(ref info);
                return;
            case LexMode.Identifier when TryScanIdentifier(ref info):
                return;
            case LexMode.ElementValue when TryScanElementValue(ref info):
                return;
        }

        if (TryScanInterpolation(ref info)) return;

        switch (Reader.Current)
        {
            case LESS_THAN_CHAR:
                Reader.Advance();
                if (Reader.Current is FORWARD_SLASH_CHAR)
                {
                    info.Kind = CXTokenKind.LessThanForwardSlash;
                    Reader.Advance();
                    return;
                }

                info.Kind = CXTokenKind.LessThan;
                return;
            case FORWARD_SLASH_CHAR when Reader.Next is GREATER_THAN_CHAR:
                Reader.Advance(2);
                info.Kind = CXTokenKind.ForwardSlashGreaterThan;
                return;
            case GREATER_THAN_CHAR:
                info.Kind = CXTokenKind.GreaterThan;
                Reader.Advance();
                return;
            case EQUALS_CHAR when Mode == LexMode.Attribute:
                info.Kind = CXTokenKind.Equals;
                Reader.Advance();
                return;
            case NULL_CHAR:
                if (Reader.IsEOF)
                {
                    info.Kind = CXTokenKind.EOF;
                    return;
                }

                goto default;

            default:
                if (Mode == LexMode.Attribute && TryScanAttributeValue(ref info)) return;

                info.Kind = CXTokenKind.Invalid;
                return;
        }
    }

    private bool TryScanElementValue(ref TokenInfo info)
    {
        var interpolationUpperBounds = InterpolationBoundary;

        var start = Reader.Position;

        for (; Reader.Position < interpolationUpperBounds; Reader.Advance())
        {
            CancellationToken.ThrowIfCancellationRequested();

            switch (Reader.Current)
            {
                case NULL_CHAR
                    or LESS_THAN_CHAR:
                    goto end;
            }
        }

        end:
        if (Reader.Position != start)
        {
            info.Kind = CXTokenKind.Text;
            return true;
        }

        return false;
    }

    private void LexStringLiteral(ref TokenInfo info)
    {
        if (QuoteChar is null)
        {
            // bad state
            throw new InvalidOperationException("Missing closing char for string literal");
        }

        if (Reader.IsEOF)
        {
            // TODO: unclosed string literal
            info.Kind = CXTokenKind.EOF;
            return;
        }

        var interpolationUpperBounds = InterpolationBoundary;

        if (Reader.Position >= interpolationUpperBounds)
        {
            if (!TryScanInterpolation(ref info))
            {
                // TODO: handle
            }

            return;
        }

        if (
            ForcedEscapedQuotes
                ? Reader.Current is BACK_SLASH_CHAR && Reader.Next == QuoteChar
                : Reader.Current == QuoteChar
        )
        {
            Reader.Advance(ForcedEscapedQuotes ? 2 : 1);

            info.Kind = CXTokenKind.StringLiteralEnd;
            QuoteChar = null;

            return;
        }

        for (; Reader.Position < interpolationUpperBounds; Reader.Advance())
        {
            CancellationToken.ThrowIfCancellationRequested();

            if (Reader.Current is BACK_SLASH_CHAR)
            {
                // escaped backslash, advance thru the current and next character
                if (Reader.Next is BACK_SLASH_CHAR && ForcedEscapedQuotes)
                {
                    Reader.Advance();
                    continue;
                }

                // is the escaped quote forced? meaning we treat it as the ending quote to the string literal
                if (QuoteChar == Reader.Next && ForcedEscapedQuotes)
                {
                    break;
                }

                // TODO: open back slash error?
            }
            else if (QuoteChar == Reader.Current) break;
        }

        // we've reached the end
        info.Kind = CXTokenKind.Text;
        return;
    }

    private bool TryScanAttributeValue(ref TokenInfo info)
    {
        if (Mode is LexMode.StringLiteral) return false;

        var isEscaped = ForcedEscapedQuotes && Reader.Current is BACK_SLASH_CHAR;

        // this is the gate for handling single vs double quotes:
        // single quotes *can not* be escaped as a valid starting
        // quote
        var quoteTestChar = isEscaped && Reader.Next is DOUBLE_QUOTE_CHAR
            ? Reader.Next
            : Reader.Current;

        if (quoteTestChar is not QUOTE_CHAR and not DOUBLE_QUOTE_CHAR)
        {
            // interpolations only
            return TryScanInterpolation(ref info);
        }

        QuoteChar = quoteTestChar;
        Reader.Advance(isEscaped ? 2 : 1);
        info.Kind = CXTokenKind.StringLiteralStart;
        return true;
    }

    private bool TryScanIdentifier(ref TokenInfo info)
    {
        var upperBounds = InterpolationBoundary;

        if (!IsValidIdentifierStartChar(Reader.Current) || Reader.Position >= upperBounds)
            return false;

        do
        {
            Reader.Advance();
        } while (IsValidIdentifierChar(Reader.Current) && Reader.Position < upperBounds &&
                 !CancellationToken.IsCancellationRequested);

        CancellationToken.ThrowIfCancellationRequested();
        info.Kind = CXTokenKind.Identifier;
        return true;


        static bool IsValidIdentifierChar(char c)
            => c is UNDERSCORE_CHAR or HYPHEN_CHAR or PERIOD_CHAR || char.IsLetterOrDigit(c);

        static bool IsValidIdentifierStartChar(char c)
            => c is UNDERSCORE_CHAR || char.IsLetter(c);
    }

    private bool TryScanInterpolation(ref TokenInfo info)
    {
        if (CurrentInterpolationSpan is { } span)
        {
            info.Kind = CXTokenKind.Interpolation;
            Reader.Advance(
                span.End - Reader.Position
            );
            InterpolationIndex = _interpolationIndex;

            return true;
        }

        return false;
    }

    private void GetTrivia(bool isTrailing, ref int trivia)
    {
        if (Mode is LexMode.StringLiteral) return;

        for (;; trivia++, Reader.Advance())
        {
            start:

            CancellationToken.ThrowIfCancellationRequested();

            var current = Reader.Current;

            if (CurrentInterpolationSpan is not null) return;

            if (IsWhitespace(current)) continue;

            if (current is CARRAGE_RETURN_CHAR && Reader.Next is NEWLINE_CHAR)
            {
                trivia += 2;
                Reader.Advance(2);

                if (isTrailing) break;

                goto start;
            }

            if (current is NEWLINE_CHAR)
            {
                if (isTrailing)
                {
                    trivia++;
                    break;
                }

                continue;
            }

            if (current is LESS_THAN_CHAR && IsCurrentlyAtCommentStart())
            {
                while (!Reader.IsEOF && !IsCurrentlAtCommentEnd() && !CancellationToken.IsCancellationRequested)
                {
                    trivia++;
                    Reader.Advance();
                }
            }

            return;
        }
    }

    private bool IsCurrentlyAtCommentStart()
        => Reader.Peek(COMMENT_START.Length) == COMMENT_START;

    private bool IsCurrentlAtCommentEnd()
        => Reader.Peek(COMMENT_END.Length) == COMMENT_END;

    private static bool IsWhitespace(char ch)
        => char.IsWhiteSpace(ch);
}
