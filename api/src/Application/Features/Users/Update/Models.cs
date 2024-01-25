namespace Application.Features.Users.Update;

public sealed record UpdateUserRequest
(
    string Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    bool IsActive,
    HashSet<string> RoleIds
);
