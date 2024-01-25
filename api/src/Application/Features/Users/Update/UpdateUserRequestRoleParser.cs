namespace Application.Features.Users.Update;

internal static class UpdateUserRequestRoleParser
{
    public static bool TryParseRoles(
        RoleInfos roleInfos,
        out IEnumerable<string> rolesToAdd,
        out IEnumerable<string> rolesToDelete)
    {
        rolesToAdd = Enumerable.Empty<string>();
        rolesToDelete = Enumerable.Empty<string>();

        var allRolesMap = roleInfos.AllRoles.ToDictionary(x => x.Id, x => x.Name);
        var rolesReverseMap = roleInfos.AllRoles.ToDictionary(x => x.Name, x => x.Id);
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

        rolesToAdd = roleInfos.RequestRoleIds
            .Select(x => allRolesMap[x])
            .Except(roleInfos.UserRoleNames);

        rolesToDelete = roleInfos.UserRoleNames
            .Where(x => !roleInfos.RequestRoleIds.Contains(rolesReverseMap[x]));

        return true;
    }
}
