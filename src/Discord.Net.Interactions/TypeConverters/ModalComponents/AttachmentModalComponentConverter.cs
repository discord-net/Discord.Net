using Discord.Interactions.TypeConverters.ModalInputs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions.TypeConverters.ModalComponents;

internal class AttachmentModalComponentConverter<T> : ModalComponentTypeConverter<T> where T : class, IAttachment
{
    public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        return TypeConverterResult.FromSuccess(option.Values.FirstOrDefault());
    }
}
