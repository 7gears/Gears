namespace Gears.Application.Features.Users;

public sealed record GetAllUsersRequest;

public sealed record GetAllUsersResponse(
    string Id,
    string Email,
    string FirstName,
    string LastName);


public sealed class GetAllUsers : Endpoint<GetAllUsersRequest, List<GetAllUsersResponse>>
{
    public override void Configure()
    {
        Get("api/users");
        AccessControl("Users-GetAll", Apply.ToThisEndpoint);
    }

    public override async Task HandleAsync(GetAllUsersRequest req, CancellationToken ct)
    {
        await SendAsync([]);
    }
}
