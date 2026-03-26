[ModalUserSelect("user-select-id")]
public IUser[] SelectedUsers { get; set; }

[ModalChannelSelect("channel-select-id")]
public IChannel[] SelectedChannels { get; set; }

[ModalRoleSelect("role-select-id")]
public IRole[] SelectedRoles { get; set; }

[ModalMentionableSelect("mentionable-select-id")]
public IMentionable[] SelectedMentionables { get; set; }