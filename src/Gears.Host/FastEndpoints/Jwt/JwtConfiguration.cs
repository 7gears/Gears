namespace Gears.Host.FastEndpoints.Jwt;

public sealed class JwtConfiguration
{
    public string Key { get; set; }
    public int DurationInSeconds { get; set; }
}