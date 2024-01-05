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
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => { options.UseNpgsql(connectionString); });

// Add services to the container.
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();

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



// Ensure the database is created.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await dbContext.Database.EnsureCreatedAsync();
}
using (var scope = app.Services.CreateScope())
{
    var userService = scope.ServiceProvider.GetRequiredService<UserService>();
    var roleService = scope.ServiceProvider.GetRequiredService<RoleService>();
}


// Use CORS with named policy
app.UseCors("OpenCORS");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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
app.MapPost("/auth/login", async (LoginRequest request, UserService userService) =>
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


app.MapPost("/users", async (User newUser, UserService userService) =>
        {
            User createdUser = await userService.CreateUserAsync(newUser);
            return Results.Created($"/users/{createdUser.Email}", createdUser);
        })
        .WithName("CreateUser")
        .WithOpenApi();

app.MapGet("/users", async (UserService userService) => await userService.GetAllUsersAsync())
        .WithName("GetAllUsers")
        .WithOpenApi();

app.MapGet("/users/{userId}", async (int userId, UserService userService) =>
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

app.MapPut("/users/{userId}", async (int userId, User updatedUser, UserService userService) =>
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
app.MapDelete("/users/{userId}", async (int userId, UserService userService) =>
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
app.MapPost("/roles", async (Role newRole, RoleService roleService) =>
        {
            Role createdRole = await roleService.CreateRoleAsync(newRole);
            return Results.Created($"/roles/{createdRole.Id}", createdRole);
        })
        .WithName("CreateRole")
        .WithOpenApi();

app.MapGet("/roles", async (RoleService roleService) => await roleService.GetAllRolesAsync())
        .WithName("GetAllRoles")
        .WithOpenApi();

app.MapGet("/roles/{roleId}", async (int roleId, RoleService roleService) =>
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

app.MapPut("/roles/{roleId}", async (int roleId, Role updatedRole, RoleService roleService) =>
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
app.MapPost("/users/{userId}/roles", async (string userId, Role newRole, UserService userService, RoleService roleService) =>
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

app.MapDelete("/users/{userId}/roles/{roleId}", async (int userId, int roleId, UserService userService, RoleService roleService) =>
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

app.Run();
