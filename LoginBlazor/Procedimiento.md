# Procedimiento:

# 1. Crear projecto Blazor WebAssembly, ASP.NET minimal API, y shared project

# 2. Instalar los paquetes necesarios para el manejo de la autenticación con JWT bearer token

Instalar `Microsoft.AspNetCore.Authentication.JwtBearer` en el Projecto de la
API

# 3. Configurar Program.cs cargando los servicios de autenticación

Adicionar al builder la autenticacion y la autorizacion

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorization();
```

inicilizar la autenticacion y la autorizacion

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

Adicionar el endpoint de autenticación

```csharp
// Login endpoint
app.MapPost("/auth/login", (LoginRequest request) =>
        {
            if (request.Email == "sury@example.com" && request.Password == "Martinez")
            {
                // JWT token generation
                var claims = new List<Claim>() { new(ClaimTypes.Name, request.Email), };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aopsjfp0aoisjf[poajsf[poajsp[fojasp[foja[psojf[paosjfp[aojsfpaojsfp[ojasf"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                        issuer: "https://www.surymartinez.com",
                        audience: "Minimal APIs Client",
                        claims: claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: credentials);

                var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

                return Results.Ok(new { AccessToken = accessToken });
            }

            return Results.BadRequest();
        })
        .WithName("Login")
        .AllowAnonymous()
        .WithOpenApi();
```

Configurar la autenticacion

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            { ValidateIssuer = true, IssuerSigningKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes("aopsjfp0aoisjf[poajsf[poajsp[fojasp[foja[psojf[paosjfp[aojsfpaojsfp[ojasf")),
            ValidIssuer = "https://www.surymartinez.com",
            ValidAudience = "Minimal APIs Client"
            };
        }
);
```

En produccion poner la informacion sobre la llave en un archivo de configuracion

Adicionar soporte para autenticacion y autorizacion en Swagger

```csharp
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
```

Adicionar politicas de autorizacion

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Name, "
