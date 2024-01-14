namespace Gears.Application.Features.Roles.GetAll;

public sealed record Response
(
    string Id,
    string Name,
    string Description,
    bool IsDefault
);
