namespace Gears.Application.Features.Account.GetProfile;

public sealed record GetProfileResponse
(
    string FirstName,
    string LastName,
    string PhoneNumber
);
