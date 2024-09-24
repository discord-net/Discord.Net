using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InteractionDataResolved :
    IResolvedDataModel,
    IModelSourceOfMultiple<IUserModel>,
    IModelSourceOfMultiple<IMemberModel>,
    IModelSourceOfMultiple<IRoleModel>,
    IModelSourceOfMultiple<IChannelModel>,
    IModelSourceOfMultiple<IMessageModel>,
    IModelSourceOfMultiple<IAttachmentModel>
{
    [JsonPropertyName("users")] public Optional<Dictionary<string, User>> Users { get; set; }

    [JsonPropertyName("members")] public Optional<Dictionary<string, GuildMember>> Members { get; set; }

    [JsonPropertyName("roles")] public Optional<Dictionary<string, Role>> Roles { get; set; }

    [JsonPropertyName("channels")] public Optional<Dictionary<string, Channel>> Channels { get; set; }

    [JsonPropertyName("messages")] public Optional<Dictionary<string, Message>> Messages { get; set; }

    [JsonPropertyName("attachments")] public Optional<Dictionary<string, Attachment>> Attachments { get; set; }

    IEnumerable<string>? IResolvedDataModel.Members => ~Members.Map(v => v.Keys);

    IEnumerable<string>? IResolvedDataModel.Roles =>  ~Roles.Map(v => v.Keys);

    IEnumerable<string>? IResolvedDataModel.Channels =>  ~Channels.Map(v => v.Keys);

    IEnumerable<string>? IResolvedDataModel.Messages =>  ~Messages.Map(v => v.Keys);

    IEnumerable<string>? IResolvedDataModel.Attachments =>  ~Attachments.Map(v => v.Keys);

    IEnumerable<string>? IResolvedDataModel.Users =>  ~Users.Map(v => v.Keys);

    IEnumerable<IUserModel> IModelSourceOfMultiple<IUserModel>.GetModels()
        => Users.Map(v => (IEnumerable<IUserModel>)v.Values) | [];

    IEnumerable<IMemberModel> IModelSourceOfMultiple<IMemberModel>.GetModels()
        => Members.Map(v => (IEnumerable<IMemberModel>)v.Values) | [];

    IEnumerable<IRoleModel> IModelSourceOfMultiple<IRoleModel>.GetModels()
        => Roles.Map(v => (IEnumerable<IRoleModel>)v.Values) | [];

    IEnumerable<IChannelModel> IModelSourceOfMultiple<IChannelModel>.GetModels()
        => Channels.Map(v => (IEnumerable<IChannelModel>)v.Values) | [];

    IEnumerable<IMessageModel> IModelSourceOfMultiple<IMessageModel>.GetModels()
        => Messages.Map(v => (IEnumerable<IMessageModel>)v.Values) | [];

    IEnumerable<IAttachmentModel> IModelSourceOfMultiple<IAttachmentModel>.GetModels()
        => Attachments.Map(v => (IEnumerable<IAttachmentModel>)v.Values) | [];

    public IEnumerable<IModel> GetDefinedModels()
    {
        if (Users.IsSpecified)
            foreach (var (_, entity) in Users.Value)
                yield return entity;

        if (Members.IsSpecified)
            foreach (var (_, entity) in Members.Value)
                yield return entity;

        if (Roles.IsSpecified)
            foreach (var (_, entity) in Roles.Value)
                yield return entity;

        if (Channels.IsSpecified)
            foreach (var (_, entity) in Channels.Value)
                yield return entity;

        if (Messages.IsSpecified)
            foreach (var (_, entity) in Messages.Value)
                yield return entity;

        if (Attachments.IsSpecified)
            foreach (var (_, entity) in Attachments.Value)
                yield return entity;
    }
}
