namespace Gears.Application.Features.Users;

public sealed record GetAllUsersRequest;

public sealed record GetAllUsersResponse;

public sealed class GetAllUsersEndpoint : Endpoint<GetAllUsersRequest, GetAllUsersResponse>
{
    public override void Configure()
    {
        Get("api/users");
    }

    public override async Task HandleAsync(GetAllUsersRequest request, CancellationToken ct)
    {
        await SendAsync(new GetAllUsersResponse());
    }
}
