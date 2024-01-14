namespace Gears.Application.Common;

public static class RoleManagerExtension
{
    public static async Task<HashSet<string>> GetRolePermissionNames(
        this RoleManager<Role> roleManager,
        Role role)
    {
        var claims = await roleManager.GetClaimsAsync(role);

        return claims
            .Where(x => x.Type == Consts.Auth.PermissionClaimType)
            .Select(x => x.Value)
            .ToHashSet();
    }

    public static async Task<bool> SaveRoleWithPermissions(
            this RoleManager<Role> roleManager,
            Role role,
            bool isNew,
            IEnumerable<string> permissionsToAdd = null,
            IEnumerable<string> permissionsToRemove = null)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            var result = isNew ?
                await roleManager.CreateAsync(role) :
                await roleManager.UpdateAsync(role);

            if (result != IdentityResult.Success)
            {
                return false;
            }

            if (permissionsToAdd != null)
            {
                foreach (var permission in permissionsToAdd)
                {
                    result = await roleManager.AddClaimAsync(role, ToClaim(permission));
                    if (result != IdentityResult.Success)
                    {
                        return false;
                    }
                }
            }

            if (permissionsToRemove != null)
            {
                foreach (var permission in permissionsToRemove)
                {
                    result = await roleManager.RemoveClaimAsync(role, ToClaim(permission));
                    if (result != IdentityResult.Success)
                    {
                        return false;
                    }
                }
            }

            scope.Complete();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static Claim ToClaim(string permission) =>
        new(Consts.Auth.PermissionClaimType, permission);
}
