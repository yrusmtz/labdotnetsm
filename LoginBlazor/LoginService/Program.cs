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

List<UserDto> userDb = new List<UserDto>();

// Add services to the container.
builder.Services.AddSingleton<UserService>(new UserService(userDb));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
            new OpenApiSecurityScheme
            { Type = SecuritySchemeType.ApiKey, In = ParameterLocation.Header, Name = HeaderNames.Authorization, Description = "Insert the token with the 'Bearer ' prefix", });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
            { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme } },
              new string[] { } } }
    );
});


// Add authentication and authorization. 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            { ValidateIssuer = true, IssuerSigningKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(
                              "aopsjfp0aoisjf[poajsf[poajsp[fojasp[foja[psojf[paosjfp[aojsfpaojsfp[ojasf")),
              ValidIssuer = "https://www.surymartinez.com", ValidAudience = "Minimal APIs Client" };
        }
);
builder.Services.AddAuthorization();

var app = builder.Build();

var userService = app.Services.GetRequiredService<UserService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Poblar la base de datos con algunos usuarios
    userService.CreateUser(new UserDto(Email: "user1@example.com", Password: "password1"));
    userService.CreateUser(new UserDto(Email: "user2@example.com", Password: "password2"));
    userService.CreateUser(new UserDto(Email: "user3@example.com", Password: "password3"));
    userService.CreateUser(new UserDto(Email: "sury@example.com", Password: "Martinez"));
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


app.MapPost("/users", (UserDto newUser) =>
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

app.MapPut("/users/{userId}", (string userId, UserDto updatedUser) =>
{
    var user = userService.UpdateUserPassword(userId, updatedUser);
    if (user == null)
    {
        return Results.NotFound($"User with ID {userId} not found.");
    }
    return Results.Ok(user);
}).WithName("UpdateUserPassword").WithOpenApi();

app.Run();
