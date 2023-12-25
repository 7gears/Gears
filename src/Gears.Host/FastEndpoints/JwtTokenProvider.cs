namespace Gears.Host.FastEndpoints;

[RegisterService<IJwtTokenProvider>(LifeTime.Scoped)]
internal sealed class JwtTokenProvider : IJwtTokenProvider
{
    private readonly JwtSettings _jwtSettings;
    private readonly TimeProvider _timeProvider;
    private readonly UserManager<User> _userManager;

    public JwtTokenProvider(
        IOptions<JwtSettings> jwtOptions,
        TimeProvider timeProvider,
        UserManager<User> userManager)
    {
        _timeProvider = timeProvider;
        _userManager = userManager;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<string> GetToken(User user)
    {
        var claims = await GetClaims(user);
        var expires = _timeProvider.GetUtcNow().AddSeconds(_jwtSettings.DurationInSeconds).DateTime;

        SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        SigningCredentials signingCredentials = new(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwtSecurityToken = new(
            claims: claims,
            expires: expires,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

    private async Task<List<Claim>> GetClaims(User user)
    {
        var claims = new List<Claim>();
        if (user.UserName != null)
        {
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        }

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }
}