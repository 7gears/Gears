namespace Gears.Application.Features.Account.UpdateProfile;

public sealed record UpdateProfileRequest
(
    string UserName,
    string FirstName,
    string LastName,
    string PhoneNumber
);
