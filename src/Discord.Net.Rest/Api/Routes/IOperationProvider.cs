namespace Discord.Rest.Api;

public interface IOperationProvider<out T> where T : IOperation
{
    T GetOperation();
}