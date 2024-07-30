using Discord.Models;

namespace Discord.Gateway.State;

internal sealed class MutableSelfUserModel(ISelfUserModel model) : ISelfUserModel
{
    public ISelfUserModel SelfUserModelPart { get; set; } = model;
    public IUserModel UserModelPart { get; set; } = model;


    ulong IEntityModel<ulong>.Id => UserModelPart.Id;

    string IUserModel.Username => UserModelPart.Username;

    string IUserModel.Discriminator => UserModelPart.Discriminator;

    string? IUserModel.GlobalName => UserModelPart.GlobalName;

    string? IUserModel.Avatar => UserModelPart.Avatar;

    bool? IUserModel.IsBot => UserModelPart.IsBot;

    bool? IUserModel.IsSystem => UserModelPart.IsSystem;

    int? IUserModel.Flags => UserModelPart.Flags;

    int? IUserModel.PublicFlags => UserModelPart.PublicFlags;

    int? ISelfUserModel.PremiumType => SelfUserModelPart.PremiumType;

    string? ISelfUserModel.Email => SelfUserModelPart.Email;

    bool? ISelfUserModel.Verified => SelfUserModelPart.Verified;

    string? ISelfUserModel.Locale => SelfUserModelPart.Locale;

    bool? ISelfUserModel.MFAEnabled => SelfUserModelPart.MFAEnabled;
}
