using LoginService;
using LoginShared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

List<User> userDb = new();
//lista de roles
List<Role> roleDb = new();

// Add services to the container.
builder.Services.AddSingleton<UserService>(new UserService(userDb));
builder.Services.AddSingleton<RoleService>(new RoleService(roleDb));

// Add CORS middleware to allow requests from all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("OpenCORS", builder =>
    {
        builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
    });
});


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
            new()
            { Type = SecuritySchemeType.ApiKey, In = ParameterLocation.Header, Name = HeaderNames.Authorization,
              Description = "Insert the token with the 'Bearer ' prefix", });

    options.AddSecurityRequirement(new()
            {
            { new()
            { Reference = new() { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme } },
              new string[] { } } }
    );
});


// Add authentication and authorization. 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters = new()
            { ValidateIssuer = true, IssuerSigningKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(
                              "aopsjfp0aoisjf[poajsf[poajsp[fojasp[foja[psojf[paosjfp[aojsfpaojsfp[ojasf")),
              ValidIssuer = "https://www.surymartinez.com", ValidAudience = "Minimal APIs Client" };
        }
);
builder.Services.AddAuthorization();

var app = builder.Build();


var userService = app.Services.GetRequiredService<UserService>();
var roleService = app.Services.GetRequiredService<RoleService>();

// Use CORS with named policy
app.UseCors("OpenCORS");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Poblar la base de datos con algunos usuarios
    
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Protected endpoint with authorization for testing purposes
app.MapGet("/protected", () => "Hello World!, you are authenticated")
        .WithName("Protected")
        .RequireAuthorization()
        .WithOpenApi();

// Login endpoint
app.MapPost("/auth/login", (LoginRequest request) =>
        {
            var user = userService.GetUserByEmail(request.Email);
            if (user != null && request.Password == user.Password)
            {
                // JWT token generation
                var claims = new List<Claim>() { new(ClaimTypes.Name, request.Email), };

                var securityKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                                "aopsjfp0aoisjf[poajsf[poajsp[fojasp[foja[psojf[paosjfp[aojsfpaojsfp[ojasf"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                        issuer: "https://www.surymartinez.com",
                        audience: "Minimal APIs Client",
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(1),
                        signingCredentials: credentials);

                var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

                return Results.Ok(new { AccessToken = accessToken });
            }

            return Results.BadRequest();
        })
        .WithName("Login")
        .AllowAnonymous()
        .WithOpenApi();


app.MapPost("/users", (User newUser) =>
{
    var createdUser = userService.CreateUser(newUser);
    return Results.Created($"/users/{createdUser.Email}", createdUser);
}).WithName("CreateUser").WithOpenApi();

app.MapGet("/users", () => userService.GetAllUsers()).WithName("GetAllUsers").WithOpenApi();

app.MapGet("/users/{userId}", (int userId) =>
{
    var user = userService.GetUserById(userId);
    if (user == null)
    {
        return Results.NotFound($"User with ID {userId} not found.");
    }

    return Results.Ok(user);
});

app.MapPut("/users/{userId}", (string userId, User updatedUser) =>
{
    var user = userService.UpdateUserPassword(userId, updatedUser);
    if (user == null)
    {
        return Results.NotFound($"User with ID {userId} not found.");
    }

    return Results.Ok(user);
}).WithName("UpdateUserPassword").WithOpenApi();

//delete user
app.MapDelete("/users/{userId}", (string userId) =>
{
    var user = userService.DeleteUser(userId);
    if (user == null)
    {
        return Results.NotFound($"User with ID {userId} not found.");
    }

    return Results.Ok(user);
}).RequireAuthorization().WithName("DeleteUser").WithOpenApi();

//endpoints para roles
app.MapPost("/roles", (Role newRole) =>
{
    var createdRole = roleService.CreateRole(newRole);
    return Results.Created($"/roles/{createdRole.Name}", createdRole);
}).WithName("CreateRole").WithOpenApi();

app.MapGet("/roles", () => roleService.GetAllRoles()).WithName("GetAllRoles").WithOpenApi();

app.MapGet("/roles/{roleName}", (string roleName) =>
{
    var role = roleService.GetRole(roleName);
    if (role == null)
    {
        return Results.NotFound($"Role with name {roleName} not found.");
    }

    return Results.Ok(role);
}).WithName("GetRole").WithOpenApi();

app.MapPut("/roles/{roleName}", (string roleName, Role updatedRole) =>
{
    var role = roleService.UpdateRole(roleName, updatedRole);
    if (role == null)
    {
        return Results.NotFound($"Role with name {roleName} not found.");
    }

    return Results.Ok(role);
}).WithName("UpdateRole").WithOpenApi();


//manejo de roles de usuario
app.MapPost("/users/{userId}/roles", (string userId, Role newRole) =>
{
    var addedRole = userService.AddRoleToUser(userId, newRole);
    if (addedRole == null)
    {
        return Results.BadRequest($"Cannot add role to user {userId}.");
    }

    return Results.Created($"/users/{userId}/roles/{addedRole.Name}", addedRole);
}).WithName("AddRoleToUser").WithOpenApi();

app.MapDelete("/users/{userId}/roles/{roleName}", (string userId, string roleName) =>
{
    var removedRole = userService.RemoveRoleFromUser(userId, roleName);
    if (removedRole == null)
    {
        return Results.NotFound($"Cannot remove role {roleName} from user {userId}.");
    }

    return Results.Ok(removedRole);
}).WithName("RemoveRoleFromUser").WithOpenApi();
app.Run();
