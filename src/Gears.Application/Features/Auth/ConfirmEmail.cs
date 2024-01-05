﻿namespace Gears.Application.Features.Auth;

using ConfirmEmailResponseResultType = Results<Ok, NotFound, UnprocessableEntity>;

public sealed record ConfirmEmailRequest(
    string Id,
    string Token
);

public sealed class ConfirmEmailRequestValidator : Validator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required");
    }
}

public sealed class ConfirmEmail(
    UserManager<User> userManager
)
    : Endpoint<ConfirmEmailRequest, ConfirmEmailResponseResultType>
{
    public override void Configure()
    {
        Post("api/auth/confirm-email");
        AllowAnonymous();
    }

    public override async Task<ConfirmEmailResponseResultType> ExecuteAsync(
        ConfirmEmailRequest request,
        CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await userManager.ConfirmEmailAsync(user, request.Token);

        return result.Succeeded ? Ok() : UnprocessableEntity();
    }
}

public sealed class ConfirmEmailSwaggerSummary : Summary<ConfirmEmail>
{
    public ConfirmEmailSwaggerSummary()
    {
        Response();
        Response(400);
        Response(404);
        Response(422);
    }
}
