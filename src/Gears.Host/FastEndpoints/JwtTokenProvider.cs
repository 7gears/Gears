﻿namespace Gears.Host.FastEndpoints;

[RegisterService<IJwtTokenProvider>(LifeTime.Scoped)]
internal sealed class JwtTokenProvider
(
    IOptions<JwtSettings> jwtOptions,
    TimeProvider timeProvider,
    UserManager<User> userManager,
    RoleManager<Role> roleManager
)
    : IJwtTokenProvider
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<string> GetToken(User user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var claims = GetClaims(user);
        var permissions = await GetPermissions([.. roles]);

        var expireAt = timeProvider.GetUtcNow().AddSeconds(_jwtSettings.DurationInSeconds).DateTime;

        var token = JWTBearer.CreateToken(
            signingKey: _jwtSettings.Key,
            expireAt: expireAt,
            claims: claims,
            roles: roles,
            permissions: permissions
        );

        return token;
    }

    private static IEnumerable<Claim> GetClaims(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id)
        };

        if (user.UserName != null)
        {
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        }

        return claims;
    }

    private async Task<IEnumerable<string>> GetPermissions(HashSet<string> roles)
    {
        if (roles.Contains(Consts.Auth.RootRole))
        {
            return Allow.AllPermissions()
                .Where(x => !string.Equals(x.PermissionName, "Descriptions", StringComparison.Ordinal))
                .Select(x => x.PermissionCode);
        }


        var map = Allow.AllPermissions()
            .ToDictionary(x => x.PermissionName, x => x.PermissionCode);

        var permissions = new HashSet<string>();
        var userRoles = roleManager.Roles.Where(x => roles.Contains(x.Name));
        foreach (var userRole in userRoles)
        {
            var roleClaims = await roleManager.GetClaimsAsync(userRole);
            foreach (var roleClaim in roleClaims)
            {
                if (roleClaim.Type == Consts.Auth.PermissionClaimType && map.TryGetValue(roleClaim.Value, out var code))
                {
                    permissions.Add(code);
                }
            }
        }

        return permissions;
    }
}
