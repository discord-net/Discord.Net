using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed class ComponentParser
{
    private const string COMMENT_START = "<!--";
    private const string COMMENT_END = "-->";

    private const char NULL_CHAR = '\0';
    private const char NEWLINE_CHAR = '\n';
    private const char CARRAGE_RETURN_CHAR = '\r';

    private const char UNDERSCORE_CHAR = '_';
    private const char HYPHEN_CHAR = '-';
    private const char PERIOD_CHAR = '.';

    private const char TAG_OPEN_CHAR = '<';
    private const char TAG_CLOSE_CHAR = '>';
    private const char FORWARD_SLASH_CHAR = '/';
    private const char BACK_SLASH_CHAR = '\\';

    private const char EQUALS_CHAR = '=';
    private const char QUOTE_CHAR = '\'';
    private const char DOUBLE_QUOTE_CHAR = '"';

    /// <summary>
    ///     the raw source, in its entirety.
    /// </summary>
    private readonly string _source;

    /// <summary>
    ///     the slices that make up the source, as presented by the source generator,
    ///     with each slice representing the boundary between interpolations.
    /// </summary>
    private readonly string[] _sourceSlices;

    /// <summary>
    ///     The source length of each interpolation, as described by the C# source.
    /// </summary>
    private readonly int[] _interpolationLengths;

    /// <summary>
    ///     The offsets of the interpolations, accumulated over the entire source.
    /// </summary>
    private readonly int[] _interpolationOffsets;

    /// <summary>
    ///     A flag-set, containing which interpolations have been processed by the
    ///     parser, since each interpolation has a width of zero within the
    ///     <see cref="_source"/>.
    /// </summary>
    private readonly bool[] _handledInterpolations;

    /// <summary>
    ///     <see langword="true"/> if the parser is at the end of the source; otherwise
    ///     <see langword="false"/>.
    /// </summary>
    private bool IsEOF => _position >= _source.Length;

    /// <summary>
    ///     Gets the current character the parser is parsing; <see cref="NULL_CHAR"/> if
    ///     the parser is at or past the end of the <see cref="_source"/>.
    /// </summary>
    private char Current => IsEOF ? NULL_CHAR : _source[_position];

    /// <summary>
    ///     Gets the next character the parser will parse; <see cref="NULL_CHAR"/> if
    ///     the next character to be parsed is at or past the end of the <see cref="_source"/>.
    /// </summary>
    private char Next => _position + 1 >= _source.Length ? NULL_CHAR : _source[_position + 1];

    /// <summary>
    ///     Gets the previous character the parser has parsed; <see cref="NULL_CHAR"/> if
    ///     the previous character doesn't exist within the bounds of the <see cref="_source"/>.
    /// </summary>
    private char Previous => _position == 0 || _position > _source.Length ? NULL_CHAR : _source[_position - 1];

    /// <summary>
    ///     Gets the current location the parser is at as a <see cref="SourceLocation"/>.
    /// </summary>
    private SourceLocation CurrentLocation => new(_line, _column, _position);


    /// <summary>
    ///     The current position (offset) the parser is at.
    /// </summary>
    private int _position;

    /// <summary>
    ///     The current zero-based line the parser is at.
    /// </summary>
    private int _line;

    /// <summary>
    ///     The current zero-based column the parser is at.
    /// </summary>
    private int _column;

    /// <summary>
    ///     A collection of diagnostics that the parser has reported.
    /// </summary>
    private readonly List<CXmlDiagnostic> _diagnostics;

    private ComponentParser(string[] slices, int[] interpolationLengths)
    {
        _source = string.Join(string.Empty, slices);
        _sourceSlices = slices;
        _interpolationLengths = interpolationLengths;

        _diagnostics = [];

        _interpolationOffsets = new int[slices.Length - 1];
        _handledInterpolations = new bool[interpolationLengths.Length];

        for (int i = 0, offset = 0; i < slices.Length - 1; i++)
        {
            _interpolationOffsets[i] = offset + slices[i].Length;
            offset += slices[i].Length;
        }
    }

    /// <summary>
    ///     Parses a <see cref="CXmlDoc"/> given the slices that make up the cxml, and the interpolation lengths.
    /// </summary>
    /// <param name="slices">The string slices that make up the cxml.</param>
    /// <param name="interpolationLengths">The length of each interpolation, as defined in C# source.</param>
    /// <returns></returns>
    public static CXmlDoc Parse(string[] slices, int[] interpolationLengths)
    {
        var parser = new ComponentParser(slices, interpolationLengths);
        var elements = new List<CXmlElement>();

        while (parser.IsElement())
        {
            elements.Add(parser.ParseElement());
        }

        return new CXmlDoc(
            (default, parser.CurrentLocation),
            elements,
            parser._interpolationOffsets,
            parser._diagnostics
        );
    }

    /// <summary>
    ///     Reports an error as a <see cref="CXmlDiagnostic"/> using the <see cref="CurrentLocation"/> as
    ///     the diagnostics position.
    /// </summary>
    /// <param name="message">The error message to report.</param>
    /// <returns>The <see cref="CXmlDiagnostic"/> that represents the error reported.</returns>
    private CXmlDiagnostic ReportError(string message)
        => ReportDiagnostic(DiagnosticSeverity.Error, message, (CurrentLocation, CurrentLocation));

    /// <summary>
    ///      Reports an error as a <see cref="CXmlDiagnostic"/> at the specified <see cref="SourceSpan"/>.
    /// </summary>
    /// <param name="message">The error message to report.</param>
    /// <param name="span">The span describing where the error is.</param>
    /// <returns>The <see cref="CXmlDiagnostic"/> that represents the error reported.</returns>
    private CXmlDiagnostic ReportError(string message, SourceSpan span)
        => ReportDiagnostic(DiagnosticSeverity.Error, message, span);

    /// <summary>
    ///     Reports a <see cref="CXmlDiagnostic"/> given the <see cref="DiagnosticSeverity"/>,
    ///     <paramref name="message"/>, and <see cref="SourceSpan"/>.
    /// </summary>
    /// <param name="severity">The severity of the diagnostic.</param>
    /// <param name="message">The human-readable message to report.</param>
    /// <param name="span">The span describing where the diagnostic is.</param>
    /// <returns>The <see cref="CXmlDiagnostic"/> that represents the error reported.</returns>
    private CXmlDiagnostic ReportDiagnostic(DiagnosticSeverity severity, string message, SourceSpan span)
    {
        var diagnostic = new CXmlDiagnostic(severity, message, span);
        _diagnostics.Add(diagnostic);
        return diagnostic;
    }

    /// <summary>
    ///     Determines whether the current location contains the start of an element.
    /// </summary>
    /// <returns>
    ///     <see langword="true"/> if the current location contains the start of an
    ///     element; otherwise <see langword="false"/>.
    /// </returns>
    private bool IsElement()
    {
        if (Current is TAG_OPEN_CHAR) return true;

        // we might have some trivia before
        var location = CurrentLocation;
        SkipWhitespace();
        var isStartElement = Current is TAG_OPEN_CHAR;
        Rollback(location);

        return isStartElement;
    }

    /// <summary>
    ///     Parses a <see cref="CXmlElement"/> from the <see cref="_source"/> at the given <see cref="_position"/> and
    ///     advances the parse state.
    /// </summary>
    /// <returns>
    ///     A <see cref="CXmlElement"/> representing the element parsed at the current <see cref="_position"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     No valid element exists within the current parse state.
    /// </exception>
    private CXmlElement ParseElement()
    {
        SkipWhitespace();

        var startLocation = CurrentLocation;
        Eat(TAG_OPEN_CHAR);

        var tagName = ParseTagName();

        var attributes = new List<CXmlAttribute>();
        var children = new List<ICXml>();
        var diagnostics = new List<CXmlDiagnostic>();

        // check for attributes
        if (IsLikelyAttribute())
        {
            attributes.AddRange(ParseAttributes());
        }

        SkipWhitespace();

        // element close
        if (Current is FORWARD_SLASH_CHAR)
        {
            Eat(FORWARD_SLASH_CHAR);
            SkipWhitespace();

            if (Current is TAG_CLOSE_CHAR)
            {
                Eat(TAG_CLOSE_CHAR);

                // empty element
                return new CXmlElement(
                    (startLocation, CurrentLocation),
                    tagName,
                    BuildAttributes(),
                    children,
                    diagnostics
                );
            }

            diagnostics.Add(
                ReportError($"Expected '{TAG_CLOSE_CHAR}', got '{Current}'")
            );
        }
        else if (Current is TAG_CLOSE_CHAR)
        {
            Eat(TAG_CLOSE_CHAR);

            // parse children next
            while (true)
            {
                var location = CurrentLocation;

                SkipWhitespace();

                var hasInterpolationBetweenWhiteSpace = IsInterpolationBetween(location.Offset, _position);

                if (IsEOF)
                {
                    diagnostics.Add(
                        ReportError(
                            $"Missing closing tag, expected '{tagName}', got EOF",
                            tagName.Span
                        )
                    );
                    break;
                }

                if (Current is TAG_OPEN_CHAR && !hasInterpolationBetweenWhiteSpace)
                {
                    Eat(TAG_OPEN_CHAR);
                    SkipWhitespace();

                    if (Current is FORWARD_SLASH_CHAR)
                    {
                        Eat(FORWARD_SLASH_CHAR);
                        // check for our tag close
                        var tagCloseName = ParseTagName();

                        if (tagName.Value != tagCloseName.Value)
                        {
                            diagnostics.Add(
                                ReportError(
                                    $"Missing closing tag",
                                    tagName.Span
                                )
                            );

                            // revert back this closing tag for the parent to consume
                            Rollback(location);
                            break;
                        }

                        SkipWhitespace();
                        Eat(TAG_CLOSE_CHAR);
                        break;
                    }
                    else
                    {
                        Rollback(location);
                        // it's another tag, parse it as such
                        children.Add(ParseElement());
                    }
                }
                else
                {
                    Rollback(location);
                    children.Add(ParseValue(ValueParsingMode.ElementValue));
                }
            }
        }
        else
        {
            diagnostics.Add(
                ReportError(
                    "Missing closing tag",
                    (startLocation, CurrentLocation)
                )
            );
        }

        return new CXmlElement(
            (startLocation, CurrentLocation),
            tagName,
            BuildAttributes(),
            children,
            diagnostics
        );

        IReadOnlyDictionary<string, CXmlAttribute> BuildAttributes()
        {
            var result = new Dictionary<string, CXmlAttribute>();

            foreach (var attribute in attributes)
            {
                if (result.ContainsKey(attribute.Name.Value))
                {
                    diagnostics.Add(
                        ReportError($"Duplicate attribute '{attribute.Name}'", attribute.Span)
                    );
                    continue;
                }

                result.Add(attribute.Name.Value, attribute);
            }

            return result;
        }
    }

    /// <summary>
    ///     Parses a set of attributes at the current <see cref="_position"/> and advances
    ///     the parse state.
    /// </summary>
    /// <returns>
    ///     An enumerable representing the parsing of the attributes at the current <see cref="_position"/>.
    /// </returns>
    private IEnumerable<CXmlAttribute> ParseAttributes()
    {
        while (IsLikelyAttribute())
            yield return ParseAttribute();
    }

    /// <summary>
    ///     Determines if the current <see cref="_position"/> within the <see cref="_source"/> is most likely
    ///     an attribute.
    /// </summary>
    /// <returns>
    ///     <see langword="true"/> if the current parse state is most likely at an attribute; otherwise
    ///     <see langword="false"/>.
    /// </returns>
    private bool IsLikelyAttribute()
    {
        SkipWhitespace();

        return Current is not TAG_CLOSE_CHAR and not FORWARD_SLASH_CHAR && IsValidNameStartChar(Current);
    }

    /// <summary>
    ///     Parses a <see cref="CXmlAttribute"/> at the current <see cref="_position"/>, and advances
    ///     the parse state.
    /// </summary>
    /// <returns>
    ///     The <see cref="CXmlAttribute"/> parsed from the <see cref="_source"/> at the current
    ///     <see cref="_position"/>.
    /// </returns>
    private CXmlAttribute ParseAttribute()
    {
        SkipWhitespace();

        var startLocation = CurrentLocation;

        var attributeName = ParseTagName();
        var nameEndLocation = CurrentLocation;

        CXmlValue? value = null;

        SkipWhitespace();

        // does it have a value?
        if (Current is EQUALS_CHAR)
        {
            Eat(EQUALS_CHAR);
            value = ParseValue(ValueParsingMode.AttributeValue);
        }

        return new CXmlAttribute(
            Span: (startLocation, value is null ? nameEndLocation : CurrentLocation),
            Name: attributeName,
            NameSpan: (startLocation, nameEndLocation),
            Value: value
        );
    }

    /// <summary>
    ///     Represents the mode on which to parse <see cref="CXmlValue"/>.
    /// </summary>
    private enum ValueParsingMode
    {
        /// <summary>
        ///     Parses values allowed inside of attributes.
        /// </summary>
        AttributeValue,

        /// <summary>
        ///     Parses values allowed inside of elements.
        /// </summary>
        ElementValue
    }

    /// <summary>
    ///     Parses a <see cref="CXmlValue"/> at the current <see cref="_position"/> using the given
    ///     <see cref="ValueParsingMode"/> and updates the parse state.
    /// </summary>
    /// <remarks>
    ///     This function will return a <see cref="CXmlValue.Invalid"/> atom in cases that no value can be parsed with
    ///     the given mode.
    /// </remarks>
    /// <param name="mode">The mode of which to parse the value.</param>
    /// <returns>
    ///     The <see cref="CXmlValue"/> parsed from the <see cref="_source"/> at the current <see cref="_position"/>.
    /// </returns>
    /// <exception cref="NotSupportedException">Unknown/invalid <see cref="ValueParsingMode"/>.</exception>
    private CXmlValue ParseValue(ValueParsingMode mode)
    {
        var diagnostics = new List<CXmlDiagnostic>();

        var startLocation = CurrentLocation;

        if (IsEOF)
        {
            diagnostics.Add(
                ReportError("Expected value, got EOF")
            );

            return new CXmlValue.Scalar((startLocation, CurrentLocation), "", Diagnostics: diagnostics);
        }

        switch (mode)
        {
            case ValueParsingMode.ElementValue:
                return ReadElementValue();

            case ValueParsingMode.AttributeValue:
                // can be quoted
                if (Current is not QUOTE_CHAR and not DOUBLE_QUOTE_CHAR)
                {
                    // check for string interpolation
                    if (IsAtStartOfInterpolation(out var interpolationIndex))
                    {
                        return new CXmlValue.Interpolation(
                            (startLocation, CurrentLocation),
                            interpolationIndex,
                            diagnostics
                        );
                    }

                    // otherwise it's an invalid attribute
                    diagnostics.Add(
                        ReportError(
                            $"Invalid attribute value: expected a quoted or interpolated string, got '{Current}'"
                        )
                    );

                    return new CXmlValue.Invalid(
                        (startLocation, CurrentLocation),
                        diagnostics
                    );
                }

                return ParseQuotedValue();

            default: throw new NotSupportedException($"Unknown value parsing mode '{mode}'");
        }

        CXmlValue ReadElementValue()
        {
            var globalStartLocation = CurrentLocation;
            var startLocation = CurrentLocation;
            var parts = new List<CXmlValue>();
            var diagnostics = new List<CXmlDiagnostic>();

            var hasOnlyWhitespace = true;

            while (true)
            {
                if (hasOnlyWhitespace && !char.IsWhiteSpace(Current))
                {
                    hasOnlyWhitespace = false;
                    globalStartLocation = CurrentLocation;
                }

                while (IsAtStartOfInterpolation(out var offsetIndex))
                {
                    // do we have any content up to this point?
                    if (startLocation.Offset != _position && !hasOnlyWhitespace)
                    {
                        parts.Add(
                            new CXmlValue.Scalar(
                                (startLocation, CurrentLocation),
                                _source.Substring(startLocation.Offset, (_position - startLocation.Offset))
                            )
                        );
                    }

                    parts.Add(
                        new CXmlValue.Interpolation(
                            (CurrentLocation, CurrentLocation),
                            offsetIndex
                        )
                    );

                    startLocation = CurrentLocation;
                    if (hasOnlyWhitespace)
                    {
                        globalStartLocation = CurrentLocation;
                        hasOnlyWhitespace = false;
                    }
                }

                if (IsEOF || Current is TAG_OPEN_CHAR) break; // we're done reading

                if (Current is NEWLINE_CHAR)
                {
                    Advance();
                    _line++;
                    _column = 0;

                    continue;
                }

                if (Current is CARRAGE_RETURN_CHAR)
                {
                    Advance();

                    var isValidFullReturn = Current is NEWLINE_CHAR;

                    if (!isValidFullReturn)
                    {
                        // TODO: report incorrect newlines
                    }
                    else Advance();

                    _line++;
                    _column = 0;
                    continue;
                }

                Advance();
            }

            if (parts.Count is 0)
            {
                // basic scalar
                return new CXmlValue.Scalar(
                    (startLocation, CurrentLocation),
                    _source.Substring(startLocation.Offset, (_position - startLocation.Offset)),
                    Diagnostics: diagnostics
                );
            }

            if (startLocation != CurrentLocation)
            {
                var remainder = _source.Substring(startLocation.Offset, (_position - startLocation.Offset));

                if (!string.IsNullOrWhiteSpace(remainder))
                {
                    // add remaining
                    parts.Add(
                        new CXmlValue.Scalar(
                            (startLocation, CurrentLocation),
                            Value: remainder
                        )
                    );
                }
            }


            if (parts.Count is 1) return parts[0];

            return new CXmlValue.Multipart(
                (globalStartLocation, CurrentLocation),
                parts,
                Diagnostics: diagnostics
            );
        }


        bool IsAtStartOfInterpolation(out int interpolationIndex)
        {
            for (interpolationIndex = 0; interpolationIndex < _interpolationOffsets.Length; interpolationIndex++)
            {
                var offset = _interpolationOffsets[interpolationIndex];

                if (offset > _position) break;

                if (offset == _position && !_handledInterpolations[interpolationIndex])
                    return _handledInterpolations[interpolationIndex] = true;
            }

            return false;
        }

        CXmlValue ParseQuotedValue()
        {
            var parts = new List<CXmlValue>();

            var quoteChar = Current;
            Eat(quoteChar);

            var valueStartLocation = CurrentLocation;

            while (true)
            {
                if (IsAtStartOfInterpolation(out var interpolationIndex))
                {
                    // is the interpolation the first part?
                    if (valueStartLocation.Offset != _position)
                    {
                        // add our current content to the parts
                        parts.Add(
                            new CXmlValue.Scalar(
                                (valueStartLocation, CurrentLocation),
                                _source.Substring(
                                    valueStartLocation.Offset,
                                    CurrentLocation.Offset - valueStartLocation.Offset
                                )
                            )
                        );
                    }

                    // add the interpolation
                    parts.Add(
                        new CXmlValue.Interpolation(
                            (CurrentLocation, CurrentLocation),
                            interpolationIndex
                        )
                    );

                    // reset the value start position
                    valueStartLocation = CurrentLocation;
                    continue;
                }

                if (Current == quoteChar)
                {
                    // is it escaped?
                    if (Previous == BACK_SLASH_CHAR)
                    {
                        Advance();
                        continue;
                    }

                    // value has ended
                    if (parts.Count > 0)
                    {
                        parts.Add(
                            new CXmlValue.Scalar(
                                (valueStartLocation, CurrentLocation),
                                _source.Substring(
                                    valueStartLocation.Offset,
                                    (CurrentLocation.Offset - valueStartLocation.Offset)
                                ),
                                Diagnostics: diagnostics
                            )
                        );
                    }

                    Advance();

                    break;
                }

                if (IsEOF)
                {
                    diagnostics.Add(
                        ReportError("Unclosed attribute value, got EOF", (startLocation, CurrentLocation))
                    );

                    break;
                }

                Advance();
            }

            if (parts.Count > 0)
            {
                return new CXmlValue.Multipart(
                    (startLocation, CurrentLocation),
                    parts,
                    quoteChar,
                    diagnostics
                );
            }

            return new CXmlValue.Scalar(
                (startLocation, CurrentLocation),
                _source.Substring(
                    startLocation.Offset + 1,
                    (CurrentLocation.Offset - startLocation.Offset - 2)
                ),
                quoteChar,
                diagnostics
            );
        }
    }

    /// <summary>
    ///     Determines whether an unhandled interpolation exists between <paramref name="start"/> and
    ///     <paramref name="end"/>.
    /// </summary>
    /// <param name="start">The lower inclusive bound.</param>
    /// <param name="end">The upper inclusive bound.</param>
    /// <returns>
    ///     <see langword="true"/> if an unhandled interpolation exists between the specified bounds; otherwise
    ///     <see langword="false"/>.
    /// </returns>
    private bool IsInterpolationBetween(int start, int end)
    {
        if (_interpolationOffsets.Length is 0) return false;

        for (var i = 0; i < _interpolationOffsets.Length; i++)
        {
            var offset = _interpolationOffsets[i];

            if (offset < start) continue;

            if (_handledInterpolations[i]) continue;

            return offset >= start && end >= offset;
        }

        return false;
    }

    /// <summary>
    ///     Parses a spec-compliant element/attribute name at the current <see cref="_position"/> and updates the
    ///     parse state.
    /// </summary>
    /// <returns>
    ///     The parsed <see cref="CXmlValue.Scalar"/> representing the tag name.
    /// </returns>
    private CXmlValue.Scalar ParseTagName()
    {
        SkipWhitespace();

        var diagnostics = new List<CXmlDiagnostic>();
        var nameStart = CurrentLocation;

        if (IsEOF)
        {
            diagnostics.Add(ReportError("Expected tag name start, got EOF"));
            return new CXmlValue.Scalar(
                (CurrentLocation, CurrentLocation),
                string.Empty,
                Diagnostics: diagnostics
            );
        }

        if (!IsValidNameStartChar(Current))
        {
            diagnostics.Add(ReportError($"Expected tag name start, got '{Current}'"));
            Advance();
            return new CXmlValue.Scalar(
                (nameStart, CurrentLocation),
                string.Empty,
                Diagnostics: diagnostics
            );
        }

        Advance();

        while (IsValidNameRestChar(Current)) Advance();

        return new CXmlValue.Scalar(
            (nameStart, CurrentLocation),
            _source.Substring(nameStart.Offset, (_position - nameStart.Offset))
        );
    }

    /// <summary>
    ///     Determines whether the supplied <see cref="char"/> is a valid starting character of a spec-compliant
    ///     element/attribute name.
    /// </summary>
    /// <param name="c">The character to validate.</param>
    /// <returns>
    ///     <see langword="true"/> if the character is a valid starting name character; otherwise
    ///     <see langword="false"/>.
    /// </returns>
    private static bool IsValidNameStartChar(char c)
        => c is UNDERSCORE_CHAR || char.IsLetter(c);

    /// <summary>
    ///     Determines whether the supplied <see cref="char"/> is a valid remaining character of a spec-compliant
    ///     element/attribute name.
    /// </summary>
    /// <param name="c">The character to validate.</param>
    /// <returns>
    ///     <see langword="true"/> if the character is a valid remaining name character; otherwise
    ///     <see langword="false"/>.
    /// </returns>
    private static bool IsValidNameRestChar(char c)
        => c is UNDERSCORE_CHAR or HYPHEN_CHAR or PERIOD_CHAR || char.IsLetterOrDigit(c);

    /// <summary>
    ///     Skips any whitespace characters, updating the parse state in the process.
    /// </summary>
    private void SkipWhitespace()
    {
        if (IsEOF) return;

        while (IsWhitespaceChar(Current))
        {
            if (Current is NEWLINE_CHAR)
            {
                Advance();

                _line++;
                _column = 0;
                continue;
            }
            else if (Current is CARRAGE_RETURN_CHAR)
            {
                var isProperFullNewline = Next is NEWLINE_CHAR;
                if (!isProperFullNewline)
                {
                    // treat as a newline anyways.
                    // TODO: figure out correct behaviour
                }

                Advance(isProperFullNewline ? 2 : 1);
                _line++;
                _column = 0;
                continue;
            }

            Advance();
        }
    }

    /// <summary>
    ///     Determines whether the supplied <see cref="char"/> is a whitespace character.
    /// </summary>
    /// <param name="c">The character to verify.</param>
    /// <returns>
    ///     <see langword="true"/> if the supplied <see cref="char"/> is a whitespace character; otherwise
    ///     <see langword="false"/>.
    /// </returns>
    private static bool IsWhitespaceChar(char c)
        => char.IsWhiteSpace(c);

    /// <summary>
    ///     Advances the parser if and only if the <see cref="Current"/> character is equal to the supplied
    ///     <see cref="char"/>; otherwise a <see cref="InvalidOperationException"/> is raised.
    /// </summary>
    /// <param name="c">The character expected to be at the current <see cref="_position"/>.</param>
    /// <exception cref="InvalidOperationException">
    ///     The current character was not the supplied character.
    /// </exception>
    private void Eat(char c)
    {
        if (IsEOF) throw new InvalidOperationException($"Expected '{c}', got EOF");

        if (Current != c) throw new InvalidOperationException($"Expected '{c}', got {Current}");

        Advance();
    }

    /// <summary>
    ///     Rolls back the parser to the specified <see cref="SourceLocation"/>.
    /// </summary>
    /// <param name="location">
    ///     The location on which to roll back to.
    /// </param>
    private void Rollback(SourceLocation location)
    {
        _position = location.Offset;
        _line = location.Line;
        _column = location.Column;
    }

    /// <summary>
    ///     Advances the parser '<paramref name="c"/>' characters.
    /// </summary>
    /// <remarks>This method does <b>NOT</b> update the <see cref="_line"/> field.</remarks>
    /// <param name="c">The number of characters to advance by</param>
    private void Advance(int c = 1)
    {
        _position += c;
        _column += c;
    }

    /// <summary>
    ///     Extracts a slice representing the bounds from the <see cref="_position"/> with the provided
    ///     <paramref name="count"/>.
    /// </summary>
    /// <param name="count">The number of characters to slice.</param>
    /// <returns>
    ///     A string representing the characters from the current position with the size of <paramref name="count"/>.
    /// </returns>
    private string GetSlice(int count)
    {
        var sz = Math.Min(count, _source.Length - _position);
        return _source.Substring(_position, sz);
    }
}
