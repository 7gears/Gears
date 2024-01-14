namespace Gears.Application.Features.Users.Add;

public sealed record Request
(
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    HashSet<string> RoleIds
);

public sealed record Response
(
    string Id
);

public sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Email)
            .IsNotEmpty()
            .EmailAddress();
    }
}
