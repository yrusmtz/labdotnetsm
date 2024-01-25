using LoginService;
using LoginShared;
using LoginShared.Security.DTOs;
using LoginShared.Security.Entities;
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

builder.Services.AddDbContext<AppDbContext>(options => { options.UseSqlServer(connectionString); });

// Add services to the container.
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<UserRoleService>();
builder.Services.AddScoped<PantallaService>();
builder.Services.AddScoped<PantallaRoleService>();
builder.Services.AddScoped<PatrocinadorRoleService>();
builder.Services.AddScoped<SucursalRoleService>();

// Add CORS middleware to allow requests from all originss
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

    var userService = scope.ServiceProvider.GetRequiredService<UserService>();
    var roleService = scope.ServiceProvider.GetRequiredService<RoleService>();
    var userRoleService = scope.ServiceProvider.GetRequiredService<UserRoleService>();
    var pantallasService = scope.ServiceProvider.GetRequiredService<PantallaService>();
    var pantallaRoleService = scope.ServiceProvider.GetRequiredService<PantallaRoleService>();
    var patrocinadorRoleService = scope.ServiceProvider.GetRequiredService<PatrocinadorRoleService>();
    var sucursalRoleService = scope.ServiceProvider.GetRequiredService<SucursalRoleService>();
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
app.MapPost("/auth/login", async (LoginRequest request, UserService userService) =>
        {
            UserEntity user;
            try
            {
                user = await userService.TestUserPasswordAsync(request.Email);
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


app.MapPost("/users", async (CreateUserDto newUser, UserService userService) =>
        {
            GetUserDto createdUser = await userService.CreateUserAsync(newUser);
            return Results.Created($"/users/{createdUser.Email}", createdUser);
        })
        .WithName("CreateUser")
        .WithOpenApi();

app.MapGet("/users", async (UserService userService) => await userService.GetAllUsersAsync())
        .WithName("GetAllUsers")
        .WithOpenApi();

app.MapGet("/users/{userId}", async (int userId, UserService userService) =>
{
    GetUserDto user;
    try
    {
        user = await userService.GetUserByIdAsync(userId);
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

app.MapPut("/users/{userId}", async (int userId, UpdateUserDto updatedUser, UserService userService) =>
        {
            GetUserDto user;
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
app.MapPost("/roles", async (CreateRoleDto newRole, RoleService roleService) =>
        {
            var createdRole = await roleService.CreateRoleAsync(newRole);
            return Results.Created($"/roles/{createdRole.Id}", createdRole);
        })
        .WithName("CreateRole")
        .WithOpenApi();

app.MapGet("/roles", async (RoleService roleService) => await roleService.GetAllRolesAsync())
        .WithName("GetAllRoles")
        .WithOpenApi();

app.MapGet("/roles/{roleId}", async (int roleId, RoleService roleService) =>
        {
            GetRoleDto role;
            try
            {
                role = await roleService.GetRoleByIdAsync(roleId);
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

app.MapPut("/roles/{roleId}", async (int roleId, UpdateRoleDto updatedRole, RoleService roleService) =>
        {
            GetRoleDto role;
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


//manejo de roles de usuario,AddUserRoleAsync
app.MapPost("/users/{userId}/roles",
    async (string userId, AssignRoleToUserDto assignRoleToUserDto, UserService userService, RoleService roleService, UserRoleService userRoleService) =>
    {
        GetRoleDto role;
        try
        {
            role = await roleService.GetRoleByIdAsync(assignRoleToUserDto.RoleId);
        }
        catch (ArgumentException)
        {
            return Results.NotFound();
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
        GetUserDto user;
        try
        {
            user = await userService.GetUserByIdAsync(int.Parse(userId));
        }
        catch (ArgumentException)
        {
            return Results.NotFound();
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
        // Call the service and the method AddUserRoleAsync
        await userRoleService.AddUserRoleAsync(user.Id, role.Id);
        return Results.Created($"/users/{userId}/roles/{assignRoleToUserDto.RoleId}", role);
    })
    .WithName("AddRoleToUser")
    .WithOpenApi();

app.MapDelete("/users/{userId}/roles/{roleId}", async (int userId, int roleId, UserService userService, RoleService roleService, UserRoleService userRoleService) =>
        {
            GetRoleDto role;
            try
            {
                role = await roleService.GetRoleByIdAsync(roleId);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
            GetUserDto user;
            try
            {
                user = await userService.GetUserByIdAsync(userId);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
           
            await userRoleService.DeleteUserRoleAsync(user.Id, role.Id);
            

            return Results.Ok(user);
        })
        .WithName("RemoveRoleFromUser")
        .WithOpenApi();


// Get user roles by user id endpoint
app.MapGet("/users/{userId}/roles", async (int userId, UserRoleService userRoleService) =>
        {
            List<GetRoleDto> roles = await userRoleService.GetUserRolesByUserIdAsync(userId);
            return Results.Ok(roles);
        })
        .WithName("GetUserRolesByUserId")
        .WithOpenApi();


//enpoint para pantallas
app.MapGet("/pantallas", async (PantallaService pantallaService) => await pantallaService.GetAllPantallasAsync())
        .WithName("GetAllPantallas")
        .WithOpenApi();

//asignar pantallas a roles
app.MapPost("/roles/{roleId}/pantallas",
    async (string roleId, AssignPantallaToRoleDto assignPantallaToRoleDto, RoleService roleService, PantallaService pantallaService, PantallaRoleService pantallaRoleService) =>
    {
        GetPantallaDto pantalla;
        try
        {
            pantalla = await pantallaService.GetPantallaByIdAsync(assignPantallaToRoleDto.PantallaId);
        }
        catch (ArgumentException)
        {
            return Results.NotFound();
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
        GetRoleDto role;
        try
        {
            role = await roleService.GetRoleByIdAsync(int.Parse(roleId));
        }
        catch (ArgumentException)
        {
            return Results.NotFound();
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
        // Call the service and the method AddUserRoleAsync
        await pantallaRoleService.AddPantallaRoleAsync(pantalla.Id, role.Id);
        return Results.Created($"/roles/{roleId}/pantallas/{assignPantallaToRoleDto.PantallaId}", pantalla);
    })
    .WithName("AddPantallaToRole")
    .WithOpenApi();

//ENPOINT PARA GetAllPatrocinadoresAsync
app.MapGet("/patrocinadores", async (PatrocinadorService patrocinadorService) => await patrocinadorService.GetAllPatrocinadoresAsync())
        .WithName("GetAllPatrocinadores")
        .WithOpenApi();

//ENPOINT PARA GetPatrocinadorByIdAsync
app.MapPost("/patrocinadores/{patrocinadorId}/roles",
        async (string patrocinadorId, AssignPatrocinadorToRoleDto assignPatrocinadorToRoleDto, PatrocinadorService patrocinadorService, RoleService roleService, PatrocinadorRoleService patrocinadorRoleService) =>
        {
            GetRoleDto role;
            try
            {
                role = await roleService.GetRoleByIdAsync(assignPatrocinadorToRoleDto.RoleId);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
            GetPatrocinadorDto patrocinador;
            try
            {
                patrocinador = await patrocinadorService.GetPatrocinadorByIdAsync(int.Parse(patrocinadorId));
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }

            // Call the service and the method AddRolePatrocinadorAsync
            await patrocinadorRoleService.AddPatrocinadorRoleAsync(role.Id, patrocinador.Id);
            return Results.Created($"/patrocinadores/{patrocinadorId}/roles/{assignPatrocinadorToRoleDto.RoleId}", role);
        })
    .WithName("AddRoleToPatrocinador")
    .WithOpenApi();

    //ENPOINT PARA GetAllSucursalesAsync
app.MapGet("/sucursales", async (SucursalService sucursalService) => await sucursalService.GetAllSucursalesAsync())
        .WithName("GetAllSucursales")
        .WithOpenApi();

//ENPOINT PARA GetSucursalByIdAsync
app.MapPost("/sucursales/{sucursalId}/roles",
        async (string sucursalId, AssignSucursalToRoleDto assignSucursalToRoleDto, SucursalService sucursalService, RoleService roleService, SucursalRoleService sucursalRoleService) =>
        {
            GetRoleDto role;
            try
            {
                role = await roleService.GetRoleByIdAsync(assignSucursalToRoleDto.RoleId);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
            GetSucursalDto sucursal;
            try
            {
                sucursal = await sucursalService.GetSucursalByIdAsync(int.Parse(sucursalId));
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }

            // Call the service and the method AddRoleSucursalAsync
            await sucursalRoleService.AddSucursalRoleAsync(role.Id, sucursal.Id);
            return Results.Created($"/sucursales/{sucursalId}/roles/{assignSucursalToRoleDto.RoleId}", role);
        })
    .WithName("AddRoleToSucursal")
    .WithOpenApi();












app.Run();

// // Login endpoint
// app.MapPost("/auth/login", (LoginRequest request) =>
//         {
//             var user = userService.GetUserByEmail(request.Email);
//             if (user != null && request.Password == user.Password)
//             {/
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
