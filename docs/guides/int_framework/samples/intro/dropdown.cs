[ComponentInteraction("role_selection")]
public async Task RoleSelection(string[] selectedRoles)
{
    ...
}

[ComponentInteraction("role_selection_*")]
public async Task RoleSelection(string id, string[] selectedRoles)
{
    ...
}
