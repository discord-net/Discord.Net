using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord;

/// <summary>
///     Represents a <b>C</b>omma <b>S</b>eperated <b>V</b>alues string.
/// </summary>
public readonly struct CSVString
{
    /// <summary>
    ///     The string containing the comma seperated values.
    /// </summary>
    public readonly string Value;

    /// <summary>
    ///     Gets a string array formed from splitting the <see cref="Value"/> by a comma.
    /// </summary>
    public string[] Values
        => Value.Split(',');

    /// <summary>
    ///     Constructs a new <see cref="CSVString"/>.
    /// </summary>
    /// <param name="collection">A collection of strings to be joined together by a comma.</param>
    public CSVString(IEnumerable<string> collection)
    {
        Value = string.Join(",", collection);
    }

    /// <summary>
    ///     Constructs a new <see cref="CSVString"/>.
    /// </summary>
    /// <param name="value">A string containing the list of values seperated by a comma.</param>
    public CSVString(string value)
    {
        Value = value;
    }

    /// <inheritdoc/>
    public override string ToString()
        => Value;

    /// <inheritdoc/>
    public override int GetHashCode()
        => Value.GetHashCode();

    public static implicit operator CSVString(string[] collection) => new(collection);
    public static implicit operator CSVString(List<string> collection) => new(collection);
    public static implicit operator CSVString(string str) => new(str);
    public static implicit operator string(CSVString csv) => csv.Value;
}
