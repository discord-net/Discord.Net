using Discord.Models;

namespace Discord;

[AttributeUsage(AttributeTargets.Property)]
public sealed class LinkAttribute<TModel> : Attribute
    where TModel : class, IModel;