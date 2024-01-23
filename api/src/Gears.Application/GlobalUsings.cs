﻿global using System.Security.Claims;
global using System.Transactions;
global using System.Web;
global using FastEndpoints;
global using FluentValidation;
global using Gears.Application.Auth;
global using Gears.Application.Common;
global using Gears.Application.Entities;
global using Gears.Application.Infrastructure;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Options;
global using static Microsoft.AspNetCore.Http.TypedResults;

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Gears.IntegrationTests")]
[assembly: InternalsVisibleTo("Gears.UnitTests")]

namespace Gears.Application
{
    public sealed class ApplicationInfo;
}