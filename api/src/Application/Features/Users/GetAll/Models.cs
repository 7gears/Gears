namespace Application.Features.Users.GetAll;

public sealed record GetAllUsersResponse
(
    string Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    bool IsActive
);
