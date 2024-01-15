namespace Gears.Application.Features.Users.Add;

public sealed record AddUserRequest
(
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    HashSet<string> RoleIds
);

public sealed record AddUserResponse
(
    string Id
);

public sealed class RequestValidator : Validator<AddUserRequest>
{
    public RequestValidator()
    {
        RuleFor(x => x.Email)
            .IsNotEmpty()
            .EmailAddress();
    }
}
