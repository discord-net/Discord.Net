namespace Discord.Net.Hanz.Utils;

public interface IWrapper<TValue, out TSelf>
    where TSelf : IWrapper<TValue, TSelf>
{
    TValue Unwrap();
    TSelf Wrap(TValue value);
}