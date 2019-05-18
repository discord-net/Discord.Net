// Please note that the library already supports type reading
// primitive types such as bool. This example is merely used
// to demonstrate how one could write a simple TypeReader.
using Discord;
using Discord.Commands;

public class BooleanTypeReader : TypeReader
{
    public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        bool result;
        if (bool.TryParse(input, out result))
            return Task.FromResult(TypeReaderResult.FromSuccess(result));

        return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as a boolean."));
    }
}
