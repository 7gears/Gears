using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Gears.Host.Configuration.Db;

public sealed class ApplicationDbContext : IdentityDbContext<User, Role, string>, IApplicationDbContext
{
}