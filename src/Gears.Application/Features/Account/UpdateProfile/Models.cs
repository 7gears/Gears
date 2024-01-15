namespace Gears.Application.Features.Account.UpdateProfile;

public sealed record UpdateProfileRequest
(
    string FirstName,
    string LastName,
    string PhoneNumber
);
