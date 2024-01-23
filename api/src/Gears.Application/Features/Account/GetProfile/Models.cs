namespace Gears.Application.Features.Account.GetProfile;

public sealed record GetProfileResponse
(
    string Email,
    string UserName,
    string FirstName,
    string LastName,
    string PhoneNumber
);
