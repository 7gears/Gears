namespace Gears.Application.Features.Roles.Delete;

public sealed record Request
(
    string Id
);

public sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}
