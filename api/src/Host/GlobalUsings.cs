﻿global using System.Security.Claims;
global using FastEndpoints;
global using FastEndpoints.ClientGen;
global using FastEndpoints.Security;
global using FastEndpoints.Swagger;
global using Application;
global using Application.Auth;
global using Application.Entities;
global using Application.Infrastructure;
global using Host.Db;
global using Host.Db.EntityConfigurations;
global using Host.Identity;
global using Host.Mail;
global using MailKit.Net.Smtp;
global using MailKit.Security;
global using Microsoft.AspNetCore.Cors.Infrastructure;
global using Microsoft.AspNetCore.DataProtection;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Options;
global using MimeKit;
global using NSwag.CodeGeneration.TypeScript;
global using System.Reflection;

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IntegrationTests")]
[assembly: InternalsVisibleTo("UnitTests")]
