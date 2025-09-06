using System.Runtime.CompilerServices;

namespace Discord;

[InterpolatedStringHandler]
public struct DesignerInterpolationHandler
{
    private readonly object?[] _interpolatedValues;

    private int _index;

    public DesignerInterpolationHandler(int literalLength, int formattedCount)
    {
        _interpolatedValues = new object?[formattedCount];
    }

    public void AppendLiteral(string s)
    {

    }

    public void AppendFormatted<T>(T value)
    {
        _interpolatedValues[_index++] = value;
    }

    public object? GetValue(int index) => _interpolatedValues[index];
    public T? GetValue<T>(int index) => (T?)_interpolatedValues[index];
    public string? GetValueAsString(int index) => _interpolatedValues[index]?.ToString();
}
