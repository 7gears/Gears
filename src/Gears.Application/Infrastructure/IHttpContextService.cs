namespace Gears.Application.Infrastructure;

public interface IHttpContextService
{
    bool HasPermission(string permission);

    string GetOrigin();
}
