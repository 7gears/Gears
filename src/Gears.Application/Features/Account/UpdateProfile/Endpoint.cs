﻿namespace Gears.Application.Features.Account.UpdateProfile;

using Result = Results<
    Ok,
    NotFound,
    UnprocessableEntity>;

public sealed class Endpoint
(
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager
)
    : Endpoint<Request, Result>
{
    public override void Configure()
    {
        Patch("api/account/profile");
    }

    public override async Task<Result> ExecuteAsync(Request request, CancellationToken ct)
    {
        var userId = userManager.GetUserId(httpContextAccessor.HttpContext!.User);
        var user = await userManager.FindByIdAsync(userId!);
        if (user == null)
        {
            return NotFound();
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;

        var identityResult = await userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
        {
            return UnprocessableEntity();
        }

        return Ok();
    }
}
