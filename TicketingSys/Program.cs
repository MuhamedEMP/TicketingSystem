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
using TicketingSys.RoleUtils;
using TicketingSys.Service;
using TicketingSys.Settings;
using TicketingSys.Util;

// this is so .net doesnt map jwt claims to long urls 
Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); 



var builder = WebApplication.CreateBuilder(args);

// Configure PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services (if you plan to store additional user data locally)
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



builder.Services.AddAuthorization(options =>
{
    // jwt claim based policies
    options.AddPolicy("ManagerOnly", policy =>
        policy.RequireClaim("roles", "manager"));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("roles", "admin"));

    // policies which check role from db
    options.AddPolicy("AdminFromDb", policy =>
        policy.Requirements.Add(new RoleInDbRequirement("admin")));

     options.AddPolicy("HrFromDb", policy =>
        policy.Requirements.Add(new RoleInDbRequirement("hr")));

     options.AddPolicy("ItFromDb", policy =>
        policy.Requirements.Add(new RoleInDbRequirement("it")));

    options.AddPolicy("UserFromDb", policy =>
        policy.Requirements.Add(new RoleInDbRequirement("user")));

    // combined policies
    options.AddPolicy("AdminHrItFromDb", policy =>
        policy.Requirements.Add(new RoleInDbRequirement("admin", "hr", "it")));

    options.AddPolicy("HrOrIt", policy =>
        policy.Requirements.Add(new RoleInDbRequirement("hr", "it")));

    options.AddPolicy("HrOrAdmin", policy =>
        policy.Requirements.Add(new RoleInDbRequirement("hr", "admin")));

    options.AddPolicy("ItOrAdmin", policy =>
       policy.Requirements.Add(new RoleInDbRequirement("it", "admin")));

    options.AddPolicy("HrOrIT", policy =>
        policy.Requirements.Add(new RoleInDbRequirement("hr", "it")));

    options.AddPolicy("AllRoles", policy =>
        policy.Requirements.Add(new RoleInDbRequirement("hr", "it","admin", "user")));
});


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ISharedService, SharedService>();
builder.Services.AddScoped<IUserUtils, UserUtils>();
// for custom role policies
builder.Services.AddScoped<IAuthorizationHandler, RoleInDbHandler>();


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