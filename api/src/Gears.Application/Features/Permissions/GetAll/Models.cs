namespace Gears.Application.Features.Permissions.GetAll;

public sealed record Permission
(
    string Id,
    string Name,
    string VisibleName
);

public sealed record PermissionGroup(
    string GroupId,
    string GroupName,
    List<Permission> Items);
