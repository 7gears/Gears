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

    public static async Task<bool> SaveUser(
        this UserManager<User> userManager,
        User user,
        bool isNew,
        IEnumerable<string> rolesToAdd,
        IEnumerable<string> rolesToDelete)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var result = isNew ?
                await userManager.CreateAsync(user) :
                await userManager.UpdateAsync(user);

            if (result != IdentityResult.Success)
            {
                return false;
            }

            if (rolesToDelete != null)
            {
                result = await userManager.RemoveFromRolesAsync(user, rolesToDelete);
                if (result != IdentityResult.Success)
                {
                    return false;
                }
            }

            if (rolesToAdd != null)
            {
                result = await userManager.AddToRolesAsync(user, rolesToAdd);
                if (result != IdentityResult.Success)
                {
                    return false;
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

    public static async Task<bool> SaveRole(
            this RoleManager<Role> roleManager,
            Role role,
            bool isNew,
            IEnumerable<string> permissionsToAdd,
            IEnumerable<string> permissionsToDelete)
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

            if (permissionsToDelete != null)
            {
                foreach (var permission in permissionsToDelete)
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
