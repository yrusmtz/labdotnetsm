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
builder.Services.AddScoped<PatrocinadorService>();
builder.Services.AddScoped<PatrocinadorRoleService>();
builder.Services.AddScoped<SucursalService>();
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

    scope.ServiceProvider.GetRequiredService<UserService>();
    scope.ServiceProvider.GetRequiredService<RoleService>();
    scope.ServiceProvider.GetRequiredService<UserRoleService>();
    scope.ServiceProvider.GetRequiredService<PantallaService>();
    scope.ServiceProvider.GetRequiredService<PantallaRoleService>();
    scope.ServiceProvider.GetRequiredService<PatrocinadorService>();
    scope.ServiceProvider.GetRequiredService<PatrocinadorRoleService>();
    scope.ServiceProvider.GetRequiredService<SucursalService>();
    scope.ServiceProvider.GetRequiredService<SucursalRoleService>();
    
    await dbContext.Database.EnsureCreatedAsync();
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

app.MapPost("/auth/login", async (LoginRequestDto request, UserService userService) =>
        {
            UserEntity user;
            try
            {
                user = await userService.TestUserPasswordAsync(request.Username);
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


app.MapGet("/users/{userId}/roles", async (int userId, UserRoleService userRoleService) =>
        {
            List<GetRoleDto> roles = await userRoleService.GetUserRolesByUserIdAsync(userId);
            return Results.Ok(roles);
        })
        .WithName("GetUserRolesByUserId")
        .WithOpenApi();

//GetUserRoles



app.MapGet("/pantallas", async (PantallaService pantallaService) => await pantallaService.GetAllPantallasAsync())
        .WithName("GetAllPantallas")
        .WithOpenApi();

app.MapGet("/roles/{roleId}/pantallas", async (string roleId, PantallaRoleService pantallaRoleService) => await pantallaRoleService.GetPantallasByRoleIdAsync(int.Parse(roleId)))
        .WithName("GetAllPantallasByRoleId")
        .WithOpenApi();

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
        await pantallaRoleService.AddPantallaRoleAsync(pantalla.Id, role.Id);
        return Results.Created($"/roles/{roleId}/pantallas/{assignPantallaToRoleDto.PantallaId}", pantalla);
    })
    .WithName("AddPantallaToRole")
    .WithOpenApi();

app.MapGet("/patrocinadores", async (PatrocinadorService patrocinadorService) => await patrocinadorService.GetAllPatrocinadoresAsync())
        .WithName("GetAllPatrocinadores")
        .WithOpenApi();

app.MapPost("/roles/{roleId}/patrocinadores",
        async (string roleId, AssignPatrocinadorToRoleDto assignPatrocinadorToRoleDto, PatrocinadorService patrocinadorService, RoleService roleService, PatrocinadorRoleService patrocinadorRoleService) =>
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
                patrocinador = await patrocinadorService.GetPatrocinadorByIdAsync(assignPatrocinadorToRoleDto.PatrocinadorId);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }

            await patrocinadorRoleService.AddPatrocinadorRoleAsync(patrocinador.Id,role.Id);
            return Results.Created($"/roles/{assignPatrocinadorToRoleDto.RoleId}/patrocinadores/{assignPatrocinadorToRoleDto.PatrocinadorId}", role);
        })
    .WithName("AddRoleToPatrocinador")
    .WithOpenApi();

app.MapGet("/sucursales", async (SucursalService sucursalService) => await sucursalService.GetAllSucursalesAsync())
        .WithName("GetAllSucursales")
        .WithOpenApi();

app.MapGet("/roles/{roleId}/sucursales", async (string roleId, SucursalRoleService sucursalRoleService) => await sucursalRoleService.GetSucursalesByRoleIdAsync(int.Parse(roleId)))
        .WithName("GetAllSucursalesByRoleId")
        .WithOpenApi();

app.MapPost("/roles/{roleId}/sucursales",
        async (string roleId, AssignSucursalToRoleDto assignSucursalToRoleDto, SucursalService sucursalService, RoleService roleService, SucursalRoleService sucursalRoleService) =>
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
                sucursal = await sucursalService.GetSucursalByIdAsync(assignSucursalToRoleDto.SucursalId);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
            
            await sucursalRoleService.AddSucursalRoleAsync(sucursal.Id,role.Id);
            return Results.Created($"/roles/{assignSucursalToRoleDto.RoleId}/sucursales/{assignSucursalToRoleDto.SucursalId}", role);
        })
    .WithName("AddRoleToSucursal")
    .WithOpenApi();

//CreateSucursalAsync(CreateSucursalDto
app.MapPost("/sucursales", async (CreateSucursalDto newSucursal, SucursalService sucursalService) =>
        {
            GetSucursalDto createdSucursal = await sucursalService.CreateSucursalAsync(newSucursal);
            return Results.Created($"/sucursales/{createdSucursal.Id}", createdSucursal);
        })
        .WithName("CreateSucursal")
        .WithOpenApi();

//UpdateSucursalAsync(int sucursalId,UpdateSucursalDto updateSucursalDto)

app.MapPut("/sucursales/{sucursalId}", async (int sucursalId, UpdateSucursalDto updatedSucursal, SucursalService sucursalService) =>
        {
            GetSucursalDto sucursal;
            try
            {
                sucursal = await sucursalService.UpdateSucursalAsync(sucursalId, updatedSucursal);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }

            return Results.Ok(sucursal);
        })
        .WithName("UpdateSucursal")
        .WithOpenApi();


//borrarSucursalAsync(int sucursalId
app.MapDelete("/sucursales/{sucursalId}", async (int sucursalId, SucursalService sucursalService) =>
        {
            try
            {
                await sucursalService.DeleteSucursalAsync(sucursalId);
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
        .WithName("DeleteSucursal")
        .WithOpenApi();


//CreatePatrocinadorAsync(CrearPatrocinadorDto
app.MapPost("/patrocinadores", async (CreatePatrocinadorDto newPatrocinador, PatrocinadorService patrocinadorService) =>
        {
            GetPatrocinadorDto createdPatrocinador = await patrocinadorService.CreatePatrocinadorAsync(newPatrocinador);
            return Results.Created($"/patrocinadores/{createdPatrocinador.Id}", createdPatrocinador);
        })
        .WithName("CreatePatrocinador")
        .WithOpenApi();

//UpdatePatrocinadorAsync(UpdatePatrocinadorDto
app.MapPut("/patrocinadores/{patrocinadorId}", async (int patrocinadorId, UpdatePatrocinadorDto updatedPatrocinador, PatrocinadorService patrocinadorService) =>
        {
            GetPatrocinadorDto patrocinador;
            try
            {
                patrocinador = await patrocinadorService.UpdatePatrocinadorAsync(patrocinadorId, updatedPatrocinador);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }

            return Results.Ok(patrocinador);
        })
        .WithName("UpdatePatrocinador")
        .WithOpenApi();
//BorrarPatrocinadorAsync(int patrocinadorId
app.MapDelete("/patrocinadores/{patrocinadorId}", async (int patrocinadorId, PatrocinadorService patrocinadorService) =>
        {
            try
            {
                await patrocinadorService.DeletePatrocinadorAsync(patrocinadorId);
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
        .WithName("DeletePatrocinador")
        .WithOpenApi();




app.Run();
