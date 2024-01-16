﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;

namespace Gears.Application.Features.Users.Add;

using Result = Results<
    Created<AddUserResponse>,
    BadRequest,
    UnprocessableEntity>;

public sealed class AddUser
(
    IOptions<PasswordOptions> passwordOptions,
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IPasswordHasher<User> passwordHasher,
    IMailService mailService,
    IHttpContextService httpContextService,
    IEnumerable<IUserValidator<User>> userValidators
)
    : Endpoint<AddUserRequest, Result>
{
    public override void Configure()
    {
        Post("api/users");
        AccessControl("Users_Add", Apply.ToThisEndpoint);
    }

    public override async Task<Result> ExecuteAsync(AddUserRequest request, CancellationToken ct)
    {
        if (!httpContextService.HasPermission(Allow.Users_ManageRoles))
        {
            request = request with { RoleIds = null };
        }

        var roles = await roleManager.Roles.AsNoTracking()
            .ToListAsync(ct);

        if (!TryParseRoles(request, roles, out var rolesToAdd))
        {
            return BadRequest();
        }

        var password = Utils.GenerateRandomPassword(passwordOptions.Value);
        var user = new User
        {
            Email = request.Email,
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            IsActive = request.IsActive,
            EmailConfirmed = true,
            PasswordHash = passwordHasher.HashPassword(null!, password)
        };

        var result = await userManager.SaveUser(
            user,
            true,
            rolesToAdd,
            null);

        if (!result)
        {
            return UnprocessableEntity();
        }

        var link = await GenerateConfirmEmailLink(user);
        var mailRequest = new MailRequest(user.Email, "Confirm Email", link);

        _ = mailService.Send(mailRequest);

        return Created(string.Empty, new AddUserResponse(user.Id));
    }

    private static bool TryParseRoles(
        AddUserRequest request,
        List<Role> roles,
        out HashSet<string> rolesToAdd)
    {
        rolesToAdd = null;
        if (request.RoleIds == null)
        {
            return true;
        }

        var rolesMap = roles.ToDictionary(x => x.Id, x => x);

        if (!request.RoleIds.All(rolesMap.ContainsKey))
        {
            return false;
        }

        var defaultRoles = rolesMap.Values
            .Where(role => role.IsDefault)
            .Select(x => x.Name);

        rolesToAdd = [.. defaultRoles];
        foreach (var roleId in request.RoleIds)
        {
            rolesToAdd.Add(rolesMap[roleId].Name);
        }

        return true;
    }

    private async Task<string> GenerateConfirmEmailLink(User user)
    {
        var origin = httpContextService.GetOrigin();
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        UriBuilder builder = new(origin)
        {
            Path = "confirm-email"
        };
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["Id"] = user.Id;
        query["Token"] = token;
        builder.Query = query.ToString()!;

        return builder.ToString();
    }
}
