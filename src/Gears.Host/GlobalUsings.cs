global using System.IdentityModel.Tokens.Jwt;
global using System.Text;
global using System.Security.Claims;
global using Microsoft.AspNetCore.Cors.Infrastructure;
global using Microsoft.AspNetCore.DataProtection;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;

global using FastEndpoints;
global using FastEndpoints.ClientGen;
global using FastEndpoints.Security;
global using FastEndpoints.Swagger;

global using MailKit.Net.Smtp;
global using MailKit.Security;
global using MimeKit;

global using Gears.Host.Db;
global using Gears.Host.Identity;
global using Gears.Host.FastEndpoints;
global using Gears.Host.Mail;
global using Gears.Application.Entities;
global using Gears.Application.Infrastructure;
