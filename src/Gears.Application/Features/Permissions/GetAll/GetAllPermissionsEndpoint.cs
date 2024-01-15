namespace Gears.Application.Features.Permissions.GetAll;

public sealed class GetAllPermissionsEndpoint : EndpointWithoutRequest<List<PermissionGroup>>
{
    public override void Configure()
    {
        Get("api/permissions");
        AccessControl("Permissions_GetAll", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = Allow.AllNames()
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

        await SendAsync(response);
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
