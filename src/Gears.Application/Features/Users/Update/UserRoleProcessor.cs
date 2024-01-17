namespace Gears.Application.Features.Users.Update;

internal static class UserRoleProcessor
{
    public static bool TryParseRoles(
        UpdateUserRequest request,
        List<Role> allRoles,
        IList<string> userRoles,
        out HashSet<string> rolesToAdd,
        out HashSet<string> rolesToDelete)
    {
        rolesToAdd = null;
        rolesToDelete = null;
        if (request.RoleIds == null)
        {
            return true;
        }

        var allRolesMap = allRoles.ToDictionary(x => x.Id, x => x);
        var rolesReverseMap = allRoles.ToDictionary(x => x.Name, x => x.Id);
        if (!request.RoleIds.All(allRolesMap.ContainsKey))
        {
            return false;
        }

        var defaultRoleNames = allRoles
            .Where(role => role.IsDefault)
            .Select(x => x.Name)
            .ToHashSet();

        rolesToAdd = request.RoleIds
            .Select(x => allRolesMap[x].Name)
            .Union(defaultRoleNames)
            .Except(userRoles)
            .ToHashSet();

        rolesToDelete = [];
        foreach (var name in userRoles)
        {
            if (defaultRoleNames.Contains(name))
            {
                continue;
            }

            if (!rolesReverseMap.TryGetValue(name, out var id) || !request.RoleIds.Contains(id))
            {
                rolesToDelete.Add(name);
            }
        }

        return true;
    }
}
