using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace Gears.Host.FastEndpoints.Jwt;

[RegisterService<IJwtTokenProvider>(LifeTime.Scoped)]
internal sealed class JwtTokenProvider
(
    IOptions<JwtConfiguration> _jwtOptions,
    TimeProvider _timeProvider,
    UserManager<User> _userManager
) : IJwtTokenProvider
{
    private readonly JwtConfiguration _jwtConfiguration = _jwtOptions.Value;

    public async Task<string> GetToken(User user)
    {
        var claims = await GetClaims(user);
        var expires = _timeProvider.GetUtcNow().AddSeconds(_jwtConfiguration.DurationInSeconds).DateTime;

        SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
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