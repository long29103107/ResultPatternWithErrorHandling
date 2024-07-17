using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ResultPatternExample.Data;
using ResultPatternExample.Middlewares;
using ResultPatternExample.Repositories;
using ResultPatternExample.Services;
using ResultPatternExample.Validations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

const string adminRole = "admin";
const string adminUserName = "admin";
const string adminEmail = "admin@nader.com";
const string adminPassword = "1q2w3E**";

const string userRole = "user";
const string userUserName = "user";
const string userEmail = "user@nader.com";
const string userPassword = "1q2w3E**";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddValidatorsFromAssemblyContaining<TodoValidator>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("tokenAuthDB"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["JWT:SecretKey"]!;
        var validIssuer = builder.Configuration["JWT:ValidIssuer"]!;
        var validAudience = builder.Configuration["JWT:ValidAudience"]!;
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = validIssuer,
            ValidAudience = validAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/create-default-admin", async (RoleManager<IdentityRole> roleManager,
                                UserManager<IdentityUser> userManager) =>
{
    var roleCreationResult = await roleManager.CreateAsync(new(adminRole));
    IdentityUser identityUser = new(adminUserName);
    identityUser.Email = adminEmail;
    var userCreationResult = await userManager.CreateAsync(identityUser, adminPassword);
    var userAddToRoleResult = await userManager.AddToRoleAsync(identityUser, adminRole);

    return new { roleCreationResult, userCreationResult, userAddToRoleResult };
});

app.MapPost("/create-default-user", async (RoleManager<IdentityRole> roleManager,
                                UserManager<IdentityUser> userManager) =>
{
    var roleCreationResult = await roleManager.CreateAsync(new(userRole));
    IdentityUser identityUser = new(userUserName);
    identityUser.Email = userEmail;
    var userCreationResult = await userManager.CreateAsync(identityUser, userPassword);
    var userAddToRoleResult = await userManager.AddToRoleAsync(identityUser, userRole);

    return new { roleCreationResult, userCreationResult, userAddToRoleResult };
});

app.MapPost("/token-admin", async (UserManager<IdentityUser> userManager, IConfiguration configuration) =>
{
    List<Claim> claims = await _GetClaimsByUserAsync(userManager, adminUserName, adminPassword);

    if (!claims.Any())
    {
        return Results.Ok("User is not found or login failed !");
    }

    string tokenValue = _WriteToken(configuration, claims);

    return Results.Ok(tokenValue);
});

app.MapPost("/token-user", async (UserManager<IdentityUser> userManager, IConfiguration configuration) =>
{
    List<Claim> claims = await _GetClaimsByUserAsync(userManager, userUserName, userPassword);

    if(!claims.Any())
    {
        return Results.Ok("User is not found or login failed !");
    }

    string tokenValue = _WriteToken(configuration, claims);

    return Results.Ok(tokenValue);
});

app.MapGet("/secure", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = adminRole)] () =>
{
    return "I'm authorized!!";
});


app.MapControllers();

app.Run();

static async Task<List<Claim>> _GetClaimsByUserAsync(UserManager<IdentityUser> userManager, string username, string password)
{
    IdentityUser? identityUser = await userManager.FindByNameAsync(username);
    if (identityUser is null)
    {
        return new List<Claim>();
    }
    var result = await userManager.PasswordValidators[0].ValidateAsync(userManager, identityUser, userPassword);
    if (!result.Succeeded)
    {
        return new List<Claim>();
    }
    var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub,   identityUser.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, identityUser.Email!),
        new(JwtRegisteredClaimNames.Name,  identityUser.UserName!),
    };


    foreach (var role in await userManager.GetRolesAsync(identityUser))
    {
        claims.Add(new(ClaimTypes.Role, role));
    }

    return claims;
}

static string _WriteToken(IConfiguration configuration, List<Claim> claims)
{
    var secretKey = configuration["JWT:SecretKey"];
    var validIssuer = configuration["JWT:ValidIssuer"];
    var validAudience = configuration["JWT:ValidAudience"];

    SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(secretKey!));
    SigningCredentials signingCredentials = new(symmetricSecurityKey,
                                                SecurityAlgorithms.HmacSha256);
    JwtSecurityToken jwtSecurityToken = new(validIssuer,
                                            validAudience,
                                            claims,
                                            null,
                                            DateTime.UtcNow.AddHours(1),
                                            signingCredentials);

    JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

    string tokenValue = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

    return tokenValue;
}