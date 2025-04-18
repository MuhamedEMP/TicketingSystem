using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using TicketingSys.Contracts.Misc;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Middleware;
using TicketingSys.Models;
using TicketingSys.Redis;
using TicketingSys.RoleHandling.Policies;
using TicketingSys.RoleHandling.RoleHandlers;
using TicketingSys.Service;
using TicketingSys.Settings;
using TicketingSys.Util;
using TicketingSys.Utils;

// this is so .net doesnt map jwt claims to long urls 
Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); 



var builder = WebApplication.CreateBuilder(args);

// Configure PostgreSQL and logging
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging() 
           .LogTo(Console.WriteLine, LogLevel.Information); 
});

//Add Identity services (if you plan to store additional user data locally)
//builder.Services.AddIdentity<User, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();


// Register controllers for API endpoints
builder.Services.AddControllers();

// Swagger/OpenAPI (optional)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
});

// for custom role policies
builder.Services.AddScoped<IAuthorizationHandler, AdminOnlyHandler>();
builder.Services.AddScoped<IAuthorizationHandler, AdminOrDeptUserHandler>();
builder.Services.AddScoped<IAuthorizationHandler, RegularUserOnlyHandler>();
builder.Services.AddScoped<IAuthorizationHandler, DeptUserOnlyHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DepartmentUserOnly", policy =>
        policy.Requirements.Add(new DeptUserOnlyRequirement()));

    options.AddPolicy("AdminOrDepartmentUser", policy =>
        policy.Requirements.Add(new AdminOrDeptUserRequirement()));

    options.AddPolicy("AdminOnly", policy =>
        policy.Requirements.Add(new AdminOnlyRequirement()));

    options.AddPolicy("RegularUserOnly", policy =>
        policy.Requirements.Add(new RegularUserOnlyRequirement()));

});


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ISharedService, SharedService>();
builder.Services.AddScoped<IUserUtils, UserUtils>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRedisUtils, RedisUtils>();

// redis service
builder.Services.AddScoped<IUserAccessCacheService, UserAccessCacheService>();

// write roles from postgres to redis and try again on 403
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, RefreshRedisOn403>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Swagger UI for development/testing
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowReactDev");

// to return 401 when no userId in JWT
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// Enable authentication & authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// for testing
public partial class Program { }