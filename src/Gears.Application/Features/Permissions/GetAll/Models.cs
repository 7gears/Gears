namespace Gears.Application.Features.Permissions.GetAll;

public sealed record Permission
(
    string PermissionId,
    string PermissionName
);

public sealed record PermissionGroup(
    string GroupId,
    string GroupName,
    List<Permission> Items);
