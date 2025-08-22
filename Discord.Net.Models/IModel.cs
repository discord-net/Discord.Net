namespace Discord.Models;

public interface IModel;

public interface IApiModel<in TModel, out TSelf>
    where TSelf : IApiModel<TModel, TSelf>, TModel
    where TModel : IModel
{
    public static abstract TSelf From(TModel model);
}