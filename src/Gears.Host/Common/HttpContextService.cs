namespace Gears.Host.Common;

[RegisterService<IHttpContextService>(LifeTime.Scoped)]
internal sealed class HttpContextService : IHttpContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetOrigin()
    {
        return Request.Headers.Origin;
    }

    private HttpRequest Request => _httpContextAccessor.HttpContext!.Request;
}