using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal abstract class DefaultSnowflakeReader<T> : TypeReader<T>
        where T : class, ISnowflakeEntity
    {
        protected abstract Task<T> GetEntity(ulong id, IInteractionContext ctx);

        public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, string option, IServiceProvider services)
        {
            if (!ulong.TryParse(option, out var snowflake))
                return TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"{option} isn't a valid snowflake thus cannot be converted into {typeof(T).Name}");

            var result = GetEntity(snowflake, context);

            if (result is not null)
                return TypeConverterResult.FromSuccess(result);
            else
                return TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"{option} must be a valid {typeof(T).Name} snowflake to be parsed.");
        }

        public override Task<string> SerializeAsync(object obj) => Task.FromResult((obj as ISnowflakeEntity)?.Id.ToString());
    }

    internal sealed class DefaultUserReader<T> : DefaultSnowflakeReader<T>
        where T : class, IUser
    {
        protected override async Task<T> GetEntity(ulong id, IInteractionContext ctx) => await ctx.Client.GetUserAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T;
    }

    internal sealed class DefaultChannelReader<T> : DefaultSnowflakeReader<T>
        where T : class, IChannel
    {
        protected override async Task<T> GetEntity(ulong id, IInteractionContext ctx) => await ctx.Client.GetChannelAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T;
    }

    internal sealed class DefaultRoleReader<T> : DefaultSnowflakeReader<T>
        where T : class, IUser
    {
        protected override Task<T> GetEntity(ulong id, IInteractionContext ctx) => Task.FromResult(ctx.Guild?.GetRole(id) as T);
    }

    internal sealed class DefaultMessageReader<T> : DefaultSnowflakeReader<T>
        where T : class, IMessage
    {
        protected override async Task<T> GetEntity(ulong id, IInteractionContext ctx) => await ctx.Channel.GetMessageAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T;
    }
}
