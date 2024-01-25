namespace Application.Features.Roles.GetAll;

public sealed record GetAllRolesResponse
(
    string Id,
    string Name,
    string Description,
    bool IsDefault
);
