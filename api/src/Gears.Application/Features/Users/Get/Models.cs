namespace Gears.Application.Features.Users.Get;

public sealed record GetUserRequest
(
    string Id
);

public sealed record GetUserResponse
(
    string Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    bool IsActive,
    IEnumerable<string> RoleIds
);

public sealed class RequestValidator : Validator<GetUserRequest>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id).IsNotEmpty();
    }
}
