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

List<User> userDb = new List<User>();

// Add services to the container.
builder.Services.AddSingleton<UserService>(new UserService(userDb));
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
            new OpenApiSecurityScheme
            { Type = SecuritySchemeType.ApiKey, In = ParameterLocation.Header, Name = HeaderNames.Authorization,
              Description = "Insert the token with the 'Bearer ' prefix", });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
            { new OpenApiSecurityScheme
              { Reference = new OpenApiReference
                { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme } },
              new string[] { } } }
    );
});


// Add authentication and authorization. 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    "aopsjfp0aoisjf[poajsf[poajsp[fojasp[foja[psojf[paosjfp[aojsfpaojsfp[ojasf")),
            ValidIssuer = "https://www.surymartinez.com", ValidAudience = "Minimal APIs Client"
        };
    }
);
builder.Services.AddAuthorization();

var app = builder.Build();


var userService = app.Services.GetRequiredService<UserService>();
// Use CORS with named policy
app.UseCors("OpenCORS");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var roleUser = new Role("Usuario", "Usuario normal");
    var roleAdmin = new Role("Administrador", "Usuario administrador");

    // Poblar la base de datos con algunos usuarios
    userService.CreateUser(new User("User 1", "user1@example.com", "password1", "Title 1",
            new List<Role> { roleUser }));
    userService.CreateUser(
            new User("User 2", "user2@example.com", "password2", "Title 2", new List<Role> { roleAdmin }));
    userService.CreateUser(new User("User 3", "user3@example.com", "password3", "Title 3",
            new List<Role> { roleUser, roleAdmin }));
    userService.CreateUser(new User("Sury", "sury@example.com", "Martinez", "Title 4", new List<Role> { roleUser }));
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
        var user = userService.GetUser(request.Email);
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

app.MapGet("/users/{userId}", (string userId) =>
{
    var user = userService.GetUser(userId);
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


app.Run();
