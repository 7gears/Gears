namespace Application.Infrastructure;

public interface IJwtTokenProvider
{
    Task<string> GetToken(User user);
}