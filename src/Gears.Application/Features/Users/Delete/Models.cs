namespace Gears.Application.Features.Users.Delete;

public sealed record DeleteUserRequest
(
    string Id
);

public sealed class RequestValidator : Validator<DeleteUserRequest>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id).IsNotEmpty();
    }
}
