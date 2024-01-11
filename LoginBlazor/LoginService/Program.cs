using LoginService;
using LoginShared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => { options.UseNpgsql(connectionString); });

// Add services to the container.
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<UserRoleService>();

// Add CORS middleware to allow requests from all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("OpenCORS", corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin()
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

UserService userService;
RoleService roleService;
UserRoleService userRoleService;


// Ensure the database is created.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    userService = scope.ServiceProvider.GetRequiredService<UserService>();
    roleService = scope.ServiceProvider.GetRequiredService<RoleService>();
    userRoleService = scope.ServiceProvider.GetRequiredService<UserRoleService>();
    await dbContext.Database.EnsureCreatedAsync();
}

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

string GenerateJwtToken(string email)
{
    var claims = new List<Claim>() { new(ClaimTypes.Name, email), };

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

    return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
}

// Login endpoint
app.MapPost("/auth/login", async (LoginRequest request) =>
        {
            User user;
            try
            {
                user = await userService.GetUserByEmailAsync(request.Email);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }

            if (request.Password != user.Password)
            {
                return Results.Unauthorized();
            }
            // JWT token generation
            string accessToken = GenerateJwtToken(user.Email);
            return Results.Ok(new { AccessToken = accessToken });
        })
        .WithName("Login")
        .AllowAnonymous()
        .WithOpenApi();


app.MapPost("/users", async (User newUser) =>
        {
            User createdUser = await userService.CreateUserAsync(newUser);
            return Results.Created($"/users/{createdUser.Email}", createdUser);
        })
        .WithName("CreateUser")
        .WithOpenApi();

app.MapGet("/users", async () => await userService.GetAllUsersAsync())
        .WithName("GetAllUsers")
        .WithOpenApi();

app.MapGet("/users/{userId}", async (int userId) =>
{
    User user;
    try
    {
        user = await userService.GetUserByIdIfExistAsync(userId);
    }
    catch (ArgumentException)
    {
        return Results.NotFound();
    }
    catch (Exception e)
    {
        return Results.Problem(e.Message);
    }

    return Results.Ok(user);
});

app.MapPut("/users/{userId}", async (int userId, User updatedUser) =>
        {
            User user;
            try
            {
                user = await userService.UpdateUserAsync(userId, updatedUser);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }

            return Results.Ok(user);
        })
        .WithName("UpdateUserPassword")
        .WithOpenApi();

//delete user
app.MapDelete("/users/{userId}", async (int userId) =>
        {
            try
            {
                await userService.DeleteUserAsync(userId);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }

            return Results.Ok();
        })
        .RequireAuthorization()
        .WithName("DeleteUser")
        .WithOpenApi();

//endpoints para roles
app.MapPost("/roles", async (Role newRole) =>
        {
            Role createdRole = await roleService.CreateRoleAsync(newRole);
            return Results.Created($"/roles/{createdRole.Id}", createdRole);
        })
        .WithName("CreateRole")
        .WithOpenApi();

app.MapGet("/roles", async () => await roleService.GetAllRolesAsync())
        .WithName("GetAllRoles")
        .WithOpenApi();

app.MapGet("/roles/{roleId}", async (int roleId) =>
        {
            Role role;
            try
            {
                role = await roleService.GetRoleIfExistAsync(roleId);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }

            return Results.Ok(role);
        })
        .WithName("GetRole")
        .WithOpenApi();

app.MapPut("/roles/{roleId}", async (int roleId, Role updatedRole) =>
        {
            Role role;
            try
            {
                role = await roleService.UpdateRoleAsync(roleId, updatedRole);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }

            return Results.Ok(role);
        })
        .WithName("UpdateRole")
        .WithOpenApi();


//manejo de roles de usuario
app.MapPost("/users/{userId}/roles", async (string userId, Role newRole) =>
        {
            Role role;
            try
            {
                role = await roleService.GetRoleIfExistAsync(newRole.Id);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
            User user;
            try
            {
                user = await userService.GetUserByIdIfExistAsync(int.Parse(userId));
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }

            User updatedUser = user with { Roles = user.Roles.Append(role).ToList() };
            await userService.UpdateUserAsync(updatedUser.Id, updatedUser);
            return Results.Created($"/users/{userId}/roles/{newRole.Id}", newRole);
        })
        .WithName("AddRoleToUser")
        .WithOpenApi();

app.MapDelete("/users/{userId}/roles/{roleId}", async (int userId, int roleId) =>
        {
            Role role;
            try
            {
                role = await roleService.GetRoleIfExistAsync(roleId);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
            User user;
            try
            {
                user = await userService.GetUserByIdIfExistAsync(userId);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
            if (!user.Roles.Contains(role))
            {
                return Results.NotFound();
            }
            User updatedUser = user with { Roles = user.Roles.Where(r => r.Id != roleId).ToList() };
            await userService.UpdateUserAsync(updatedUser.Id, updatedUser);

            return Results.Ok(updatedUser);
        })
        .WithName("RemoveRoleFromUser")
        .WithOpenApi();

//add user to role endpoint
// Add user to role endpoint
app.MapPost("/users/{userId}/roles/{roleId}", async (int userId, int roleId) =>
    {
        await userRoleService.AddUserRoleAsync(userId, roleId);
        Role role = await roleService.GetRoleIfExistAsync(roleId);
        return Results.Created($"/users/{userId}/roles/{roleId}", role);
    })
    .WithName("AddUserToRole")
    .WithOpenApi();

// Delete user from role endpoint
app.MapDelete("/users/{userId}/roles/{roleId}", async (int userId, int roleId) =>
    {
        await userRoleService.DeleteUserRoleAsync(userId, roleId);
        return Results.NoContent();
    })
    .WithName("DeleteUserFromRole")
    .WithOpenApi();

// Get user role endpoint
app.MapGet("/users/{userId}/roles/{roleId}", async (int userId, int roleId) =>
    {
        Role role = await userRoleService.GetUserRoleAsync(userId, roleId);
        return Results.Ok(role);
    })
    .WithName("GetUserRole")
    .WithOpenApi();

// Get user roles by user id endpoint
app.MapGet("/users/{userId}/roles", async (int userId) =>
    {
        List<Role> roles = await userRoleService.GetUserRolesByUserIdAsync(userId);
        return Results.Ok(roles);
    })
    .WithName("GetUserRolesByUserId")
    .WithOpenApi();
app.Run();



// // Login endpoint
// app.MapPost("/auth/login", (LoginRequest request) =>
//         {
//             var user = userService.GetUserByEmail(request.Email);
//             if (user != null && request.Password == user.Password)
//             {
//                 // JWT token generation
//                 var claims = new List<Claim>() { new(ClaimTypes.Name, request.Email), };
//
//                 var securityKey = new SymmetricSecurityKey(
//                         Encoding.UTF8.GetBytes(
//                                 "aopsjfp0aoisjf[poajsf[poajsp[fojasp[foja[psojf[paosjfp[aojsfpaojsfp[ojasf"));
//                 var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
//
//                 var jwtSecurityToken = new JwtSecurityToken(
//                         issuer: "https://www.surymartinez.com",
//                         audience: "Minimal APIs Client",
//                         claims: claims,
//                         expires: DateTime.UtcNow.AddHours(1),
//                         signingCredentials: credentials);
//
//                 var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
//
//                 return Results.Ok(new { AccessToken = accessToken });
//             }
//
//             return Results.BadRequest();
//         })
//         .WithName("Login")
//         .AllowAnonymous()
//         .WithOpenApi();
//
//
// app.MapPost("/users", (User newUser) =>
// {
//     var createdUser = userService.CreateUser(newUser);
//     return Results.Created($"/users/{createdUser.Email}", createdUser);
// }).WithName("CreateUser").WithOpenApi();
//
// //desde aqui empiezan los endpoints de userRole
// //nuevo Endpoint POST /users/{userId}/roles/{roleId}
// app.MapPost("/userRoles", (int userId, int roleId) => { return userRoleService.AddUserRole(userId, roleId); })
//         .WithName("AddUserRole")
//         .WithOpenApi();
//
//
// //nuevo Endpoint GET /users/{userId}/roles
// app.MapGet("/userRoles/{userId}", (int userId) => { return userRoleService.GetUserRolesByUserId(userId); })
//         .WithName("GetUserRolesByUserId")
//         .WithOpenApi();
//
// //nuevo Endpoint GET /users/{userId}/roles/{roleId}
// app.MapGet("/userRoles/{userId}/{roleId}", async (int userId, int roleId) =>
// {
//     try
//     {
//         var userRole = await userRoleService.GetUserRole(userId, roleId);
//         return (userRole is null)
//                 ? Results.NotFound($"User Role with user id: {userId} and role id: {roleId} not found.")
//                 : Results.Ok(userRole);
//     }
//     catch (Exception e)
//     {
//         return Results.Problem(detail: e.Message, statusCode: 500);
//     }
// }).WithName("GetUserRole").WithOpenApi();
//
//
// //nuevo Endpoint DELETE /users/{userId}/roles/{roleId}
// app.MapDelete("/userRoles/{userId}/{roleId}",
//                 (int userId, int roleId) => { return userRoleService.DeleteUserRole(userId, roleId); })
//         .WithName("DeleteUserRole")
//         .WithOpenApi();
//
// //nuevo Endpoint PUT /users/{userId}/roles/{roleId}
// app.MapPut("/userRoles/{userId}/{roleId}",
//         (int userId, int roleId, UserRole updatedUserRole) =>
//         {
//             return userRoleService.UpdateUserRole(userId, roleId, updatedUserRole);
//         }).WithName("UpdateUserRole").WithOpenApi();
//
//
// //este no lo modifique 
// app.MapGet("/users", () => userService.GetAllUsers()).WithName("GetAllUsers").WithOpenApi();
//
// app.MapGet("/users/{userId}", (int userId) =>
// {
//     var user = userService.GetUserById(userId);
//     if (user == null)
//     {
//         return Results.NotFound($"User with ID {userId} not found.");
//     }
//
//     return Results.Ok(user);
// });
//
// app.MapPut("/users/{userId}", (int userId, User updatedUser) =>
// {
//     var user = userService.UpdateUser(userId, updatedUser);
//     if (user == null)
//     {
//         return Results.NotFound($"User with ID {userId} not found.");
//     }
//
//     return Results.Ok(user);
// }).WithName("UpdateUserPassword").WithOpenApi();
//
// //delete user
// app.MapDelete("/users/{userId}", (string userId) =>
// {
//     var user = userService.DeleteUser(userId);
//     if (user == null)
//     {
//         return Results.NotFound($"User with ID {userId} not found.");
//     }
//
//     return Results.Ok(user);
// }).RequireAuthorization().WithName("DeleteUser").WithOpenApi();
//
// //roles
// //endpoints para roles
// app.MapPost("/roles", (Role newRole) =>
// {
//     var createdRole = roleService.CreateRole(newRole);
//     return Results.Created($"/roles/{createdRole.Code}", createdRole);
// }).WithName("CreateRole").WithOpenApi();
//
// app.MapGet("/roles", () => roleService.GetAllRoles()).WithName("GetAllRoles").WithOpenApi();
//
// app.MapGet("/roles/{id:int}", (int id) =>
// {
//     var role = roleService.GetRoleById(id);
//     if (role == null)
//     {
//         return Results.NotFound($"Role with id {id} not found.");
//     }
//
//     return Results.Ok(role);
// }).WithName("GetRole").WithOpenApi();
//
// app.MapPut("/roles/{id:int}", (int id, Role updatedRole) =>
// {
//     try
//     {
//         var role = roleService.UpdateRole(id, updatedRole);
//         return Results.Ok(role);
//     }
//     catch (KeyNotFoundException)
//     {
//         return Results.NotFound($"Role with id {id} not found.");
//     }
// }).WithName("UpdateRole").WithOpenApi();
//
// app.Run();
