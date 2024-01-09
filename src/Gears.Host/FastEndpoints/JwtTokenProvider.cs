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
        var claims = await GetClaims(user);
        var expireAt = timeProvider.GetUtcNow().AddSeconds(_jwtSettings.DurationInSeconds).DateTime;

        var token = JWTBearer.CreateToken(
            signingKey: _jwtSettings.Key,
            expireAt: expireAt,
            claims: claims
        );

        return token;
    }

    private async Task<List<Claim>> GetClaims(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
        };

        if (user.UserName != null)
        {
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        }

        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }
}
