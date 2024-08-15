namespace Discord.Models;

public interface IExtendedModel<out T>
    where T : IModel
{
    T ExtendedValue { get; }

    Type ExtendedType => ExtendedValue.GetType();
}