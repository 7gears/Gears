namespace Gears.Application.Features.Users.GetAll;

public sealed record Response
(
    string Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    bool IsActive
);
