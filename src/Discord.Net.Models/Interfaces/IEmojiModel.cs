namespace Discord.Models;

[ModelEquality]
public partial interface IEmojiModel : IEmoteModel
{
    new string Name { get; }

    string? IEmoteModel.Name => Name;
}
