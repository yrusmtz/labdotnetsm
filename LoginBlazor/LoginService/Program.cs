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
using RoleService = LoginService.RoleService;
using UserService = LoginService.UserService;
using UserRoleService = LoginService.UserRoleService;


var builder = WebApplication.CreateBuilder(args);

List<User> userDb = new();
//lista de roles
List<Role> roleDb = new();

// Add services to the container.
builder.Services.AddSingleton<UserService>(new UserService(userDb));
builder.Services.AddSingleton<RoleService>(new RoleService(roleDb));
builder.Services.AddSingleton<UserRoleService>(new UserRoleService(userDb, roleDb));

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
              { Reference = new()
                { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme } },
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
var userRoleService = app.Services.GetRequiredService<UserRoleService>();

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

//desde aqui empiezan los endpoints de userRole
//nuevo Endpoint POST /users/{userId}/roles/{roleId}
app.MapPost("/userRoles", (int userId, int roleId) => { return userRoleService.AddUserRole(userId, roleId); })
        .WithName("AddUserRole")
        .WithOpenApi();


//nuevo Endpoint GET /users/{userId}/roles
app.MapGet("/userRoles/{userId}", (int userId) => { return userRoleService.GetUserRolesByUserId(userId); })
        .WithName("GetUserRolesByUserId")
        .WithOpenApi();

//nuevo Endpoint GET /users/{userId}/roles/{roleId}
app.MapGet("/userRoles/{userId}/{roleId}", async (int userId, int roleId) =>
{
    try
    {
        var userRole = await userRoleService.GetUserRole(userId, roleId);
        return (userRole is null)
                ? Results.NotFound($"User Role with user id: {userId} and role id: {roleId} not found.")
                : Results.Ok(userRole);
    }
    catch (Exception e)
    {
        return Results.Problem(detail: e.Message, statusCode: 500);
    }
}).WithName("GetUserRole").WithOpenApi();


//nuevo Endpoint DELETE /users/{userId}/roles/{roleId}
app.MapDelete("/userRoles/{userId}/{roleId}",
                (int userId, int roleId) => { return userRoleService.DeleteUserRole(userId, roleId); })
        .WithName("DeleteUserRole")
        .WithOpenApi();

//nuevo Endpoint PUT /users/{userId}/roles/{roleId}
app.MapPut("/userRoles/{userId}/{roleId}",
        (int userId, int roleId, UserRole updatedUserRole) =>
        {
            return userRoleService.UpdateUserRole(userId, roleId, updatedUserRole);
        }).WithName("UpdateUserRole").WithOpenApi();


//este no lo modifique 
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

app.MapPut("/users/{userId}", (int userId, User updatedUser) =>
{
    var user = userService.UpdateUser(userId, updatedUser);
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

//roles
//endpoints para roles
app.MapPost("/roles", (Role newRole) =>
{
    var createdRole = roleService.CreateRole(newRole);
    return Results.Created($"/roles/{createdRole.Code}", createdRole);
}).WithName("CreateRole").WithOpenApi();

app.MapGet("/roles", () => roleService.GetAllRoles()).WithName("GetAllRoles").WithOpenApi();

app.MapGet("/roles/{id:int}", (int id) =>
{
    var role = roleService.GetRoleById(id);
    if (role == null)
    {
        return Results.NotFound($"Role with id {id} not found.");
    }

    return Results.Ok(role);
}).WithName("GetRole").WithOpenApi();

app.MapPut("/roles/{id:int}", (int id, Role updatedRole) =>
{
    try
    {
        var role = roleService.UpdateRole(id, updatedRole);
        return Results.Ok(role);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound($"Role with id {id} not found.");
    }
}).WithName("UpdateRole").WithOpenApi();

app.Run();
