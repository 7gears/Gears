using Gears.Application.Auth;

namespace Gears.Application.Features.Permissions;

public sealed record GetAllPermissionsRequest;

public sealed record GetAllPermissionsResponse(
    string Code,
    string Name
);

public sealed class GetAllPermissions : Endpoint<GetAllPermissionsRequest, List<GetAllPermissionsResponse>>
{
    public override void Configure()
    {
        Get("api/permissions");
    }

    public override async Task HandleAsync(GetAllPermissionsRequest request, CancellationToken ct)
    {
        var permissions = Allow.AllPermissions()
            .Select(x => new GetAllPermissionsResponse(x.PermissionCode, x.PermissionName))
            .ToList();

        await SendAsync(permissions);
    }
}
