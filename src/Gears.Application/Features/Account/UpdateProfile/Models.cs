namespace Gears.Application.Features.Account.UpdateProfile;

public sealed record Request
(
    string FirstName,
    string LastName,
    string PhoneNumber
);
