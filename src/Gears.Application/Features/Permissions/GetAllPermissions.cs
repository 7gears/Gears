namespace Gears.Application.Features.Permissions;

public sealed record GetAllPermissionsRequest;

public sealed record Permission(
    string PermissionId,
    string PermissionName);

public sealed record PermissionGroup(
    string GroupId,
    string GroupName,
    List<Permission> Items);

public sealed class GetAllPermissions : Endpoint<GetAllPermissionsRequest, List<PermissionGroup>>
{
    public override void Configure()
    {
        Get("api/permissions");
        AccessControl("Permissions-GetAll", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(GetAllPermissionsRequest request, CancellationToken ct)
    {
        var result = Allow.AllNames()
            .Where(x => !string.Equals(x, "Descriptions", StringComparison.Ordinal))
            .Select(Split)
            .GroupBy(x => x.GroupName)
            .ToDictionary(x => x.Key, x => x.ToList())
            .Select(kvp => new
            {
                kvp,
                groupItems = kvp.Value.Select(item => new Permission(item.Name, item.ItemName)).ToList()
            })
            .Select(x => new PermissionGroup(x.kvp.Key, x.kvp.Key, x.groupItems))
            .ToList();

        await SendAsync(result);
    }

    private static PermissionsParts Split(string name)
    {
        var parts = name.Split(Consts.Auth.PermissionDelimiter, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return default;
        }

        return new(name, parts[0], parts[1]);
    }

    internal sealed record PermissionsParts(
        string Name,
        string GroupName,
        string ItemName);
}
