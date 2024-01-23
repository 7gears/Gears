namespace Gears.Application.Infrastructure;

public interface IJwtTokenProvider
{
    Task<string> GetToken(User user);
}