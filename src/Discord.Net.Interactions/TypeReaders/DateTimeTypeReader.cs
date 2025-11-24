using System;
using System.Threading.Tasks;

namespace Discord.Interactions.TypeReaders;

internal class DateTimeTypeReader : TypeReader<DateTime>
{
    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, string option, IServiceProvider services)
    {
        if (DateTime.TryParse(option, out var dateTime))
            return Task.FromResult(TypeConverterResult.FromSuccess(dateTime));

        return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"{option} is not a valid date time."));
    }
}
