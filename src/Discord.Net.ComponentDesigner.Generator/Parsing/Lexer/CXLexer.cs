using Microsoft.CodeAnalysis.Text;
using System;

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

    private struct State
    {
        public int NextInterpolationIndex;
        public int InterpolationIndex;
        public char? QuoteChar;
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
            ref var interpolationIndex = ref _state.InterpolationIndex;

            // there's no next interpolation
            if (Reader.Source.Interpolations.Length <= interpolationIndex) return null;

            for (; interpolationIndex < Reader.Source.Interpolations.Length; interpolationIndex++)
            {
                var interpolationSpan = Reader.Source.Interpolations[interpolationIndex];

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
            ref var interpolationIndex = ref _state.NextInterpolationIndex;

            // there's no next interpolation
            if (Reader.Source.Interpolations.Length <= interpolationIndex) return null;

            // check if it's ahead of us
            TextSpan? interpolationSpan = null;

            for (; interpolationIndex < Reader.Source.Interpolations.Length; interpolationIndex++)
            {
                interpolationSpan = Reader.Source.Interpolations[interpolationIndex];
                if (interpolationSpan.Value.Start > Reader.Position) break;
            }

            return interpolationSpan;
        }
    }

    private readonly bool[] _handledInterpolations;

    public LexMode Mode { get; set; }
    private State _state;

    public CXLexer(CXSourceReader reader)
    {
        Reader = reader;
        _handledInterpolations = new bool[reader.Source.Interpolations.Length];
        Mode = LexMode.Default;
        _state = default;
    }

    private void UpdateInterpolationState()
    {
        ref var interpolationIndex = ref _state.NextInterpolationIndex;
        if (Reader.Source.Interpolations.Length > interpolationIndex)
        {
            var interpolation = Reader.Source.Interpolations[interpolationIndex];

            if (Reader.Position > interpolation.End) interpolationIndex++;
        }
    }

    public CXToken Next()
    {
        InterpolationIndex = null;

        var info = default(TokenInfo);

        GetTrivia(isTrailing: false, ref info.LeadingTriviaLength);

        info.Start = Reader.Position;

        Scan(ref info);

        info.End = Reader.Position;

        GetTrivia(isTrailing: true, ref info.TrailingTriviaLength);

        return new CXToken(
            info.Kind,
            new TextSpan(info.Start, info.End - info.Start),
            info.LeadingTriviaLength,
            info.TrailingTriviaLength,
            info.Flags
        );
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
        var interpolationUpperBounds = NextInterpolationSpan?.Start ?? Reader.Source.Length;

        var start = Reader.Position;

        for (; Reader.Position < interpolationUpperBounds; Reader.Advance())
        {
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
        if (_state.QuoteChar is null)
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

        var interpolationUpperBounds = NextInterpolationSpan?.Start ?? Reader.Source.Length;

        if (Reader.Position >= interpolationUpperBounds)
        {
            if (!TryScanInterpolation(ref info))
            {
                // TODO: handle
            }

            return;
        }

        if (Reader.Current == _state.QuoteChar)
        {
            Reader.Advance();

            info.Kind = CXTokenKind.StringLiteralEnd;
            Mode = LexMode.Default;
            _state.QuoteChar = null;

            return;
        }

        for (; Reader.Position < interpolationUpperBounds; Reader.Advance())
        {
            if (_state.QuoteChar == Reader.Current)
            {
                // is it escaped?
                if (Reader.Previous is FORWARD_SLASH_CHAR)
                {
                    // allow
                    continue;
                }

                // we've reached the end
                info.Kind = CXTokenKind.Text;
                return;
            }
        }
    }

    private bool TryScanAttributeValue(ref TokenInfo info)
    {
        if (Mode is LexMode.StringLiteral) return false;

        if (Reader.Current is not QUOTE_CHAR and not DOUBLE_QUOTE_CHAR)
        {
            // interpolations only
            return TryScanInterpolation(ref info);
        }

        _state.QuoteChar = Reader.Current;
        Reader.Advance();
        info.Kind = CXTokenKind.StringLiteralStart;
        Mode = LexMode.StringLiteral;
        return true;
    }

    private bool TryScanIdentifier(ref TokenInfo info)
    {
        var upperBounds = NextInterpolationSpan?.Start ?? Reader.Source.Length;

        if (!IsValidIdentifierStartChar(Reader.Current) || Reader.Position >= upperBounds)
            return false;

        do
        {
            Reader.Advance();
        } while (IsValidIdentifierChar(Reader.Current) && Reader.Position < upperBounds);

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
            InterpolationIndex = _state.InterpolationIndex;
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

            return;
        }
    }

    private static bool IsWhitespace(char ch)
        => char.IsWhiteSpace(ch);
}
