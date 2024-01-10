namespace Gears.Host.FastEndpoints;

[RegisterService<IJwtTokenProvider>(LifeTime.Scoped)]
internal sealed class JwtTokenProvider(
    IOptions<JwtSettings> jwtOptions,
    TimeProvider timeProvider,
    UserManager<User> userManager
)
    : IJwtTokenProvider
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<string> GetToken(User user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var claims = GetClaims(user);
        var permissions = GetPermissions([.. roles]);

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

    private static IEnumerable<string> GetPermissions(HashSet<string> roles)
    {
        if (roles.Contains(Consts.Auth.RootRole))
        {
            return Allow.AllCodes();
        }

        return Enumerable.Empty<string>();
    }
}
