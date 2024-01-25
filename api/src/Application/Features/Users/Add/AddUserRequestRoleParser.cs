namespace Application.Features.Users.Add;

internal static class AddUserRequestRoleParser
{
    public static bool TryParseRoles(
        RoleInfos roleInfos,
        out IEnumerable<string> rolesToAdd)
    {
        rolesToAdd = Enumerable.Empty<string>();

        var allRolesMap = roleInfos.AllRoles.ToDictionary(x => x.Id, x => x.Name);
        var defaultRoleIds = roleInfos.AllRoles
            .Where(x => x.IsDefault)
            .Select(x => x.Id);

        if (!roleInfos.RequestRoleIds.All(allRolesMap.ContainsKey))
        {
            return false;
        }

        if (!defaultRoleIds.All(roleInfos.RequestRoleIds.Contains))
        {
            return false;
        }

        rolesToAdd = roleInfos.RequestRoleIds.Select(x => allRolesMap[x]);

        return true;
    }
}
