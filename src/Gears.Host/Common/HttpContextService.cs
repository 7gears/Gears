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
        var result = Request.Headers.Origin;
        if (string.IsNullOrEmpty(result))
        {
            result = $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
        }

        return result;
    }

    public bool HasPermission(string permission)
    {
        var claim = _httpContextAccessor.HttpContext?.User
            .FindAll(Consts.Auth.PermissionClaimType)
            .FirstOrDefault(x => x.Value == permission);

        return claim != null;
    }

    private HttpRequest Request => _httpContextAccessor.HttpContext!.Request;
}
