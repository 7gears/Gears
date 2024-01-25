global using System.Security.Claims;
global using System.Transactions;
global using System.Web;
global using FastEndpoints;
global using FluentValidation;
global using Application.Auth;
global using Application.Common;
global using Application.Entities;
global using Application.Infrastructure;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Options;

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IntegrationTests")]
[assembly: InternalsVisibleTo("UnitTests")]

namespace Application
{
    public sealed class ApplicationInfo
    {
    }
}
