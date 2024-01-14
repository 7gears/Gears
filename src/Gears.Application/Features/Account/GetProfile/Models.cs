namespace Gears.Application.Features.Account.GetProfile;

public sealed record Response
(
    string FirstName,
    string LastName,
    string PhoneNumber
);
