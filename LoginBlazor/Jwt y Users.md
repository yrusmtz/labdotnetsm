# Funcionamiento de JWT y Users

## JWT

### ¿Qué es JWT?
JWT (por sus siglas en inglés JSON Web Token) es un estándar que define una manera segura de transmitir información entre dos partes a través de JSON. Los JWT son especialmente populares en los procesos de autentificación.

### Estructura JSON Web Token
Un JWT firmado consta de 3 partes separadas por puntos.

![Estructura JWT](https://res.cloudinary.com/practicaldev/image/fetch/s--MTT2QHbt--/c_limit%2Cf_auto%2Cfl_progressive%2Cq_auto%2Cw_880/https://dev-to-uploads.s3.amazonaws.com/uploads/articles/04bibz8lka7qdx5f33oh.png)


- HEADER
Consta generalmente de dos valores y proporciona información importante sobre el token. Contiene el tipo de token y el algoritmo de la firma.

- PAYLOAD
Contiene la información real que se transmitirá a la aplicación. Aquí se definen algunos estándares que determinan qué datos se transmiten y cómo. La información se proporciona como pares Key/Value (Clave/Valor). Las claves se denominan claims en JWT. Hay tres tipos diferentes de claims (registrados, públicos, privados).

- SIGNATURE
La firma de un JSON Web Token se crea utilizando la codificación Base64 del header y del payload, así como el método de firma o cifrado especificado.


### Flujo basico de JWT
![Flujo JWt](https://res.cloudinary.com/practicaldev/image/fetch/s--7Hk1iVWA--/c_limit%2Cf_auto%2Cfl_progressive%2Cq_auto%2Cw_880/https://dev-to-uploads.s3.amazonaws.com/uploads/articles/04ttxv7nmisvs5gnd0i9.png)

https://dev.to/gdcodev/introduccion-a-json-web-token-3mjf

___
## Login y JWT en la API
La generación de JWT (JSON Web Tokens) en este archivo se realiza en el endpoint de inicio de sesión ("/auth/login"). Aquí están los pasos detallados:

1. Primero, se obtiene el usuario de la base de datos utilizando el correo electrónico proporcionado en la solicitud de inicio de sesión.

```csharp
var user = userService.GetUser(request.Email);
```

2. Luego, se verifica si el usuario existe y si la contraseña proporcionada coincide con la del usuario.

```csharp
if (user != null && request.Password == user.Password)
```

3. Si la verificación es exitosa, se procede a la generación del JWT. Primero, se crean las reclamaciones (claims) que se incluirán en el token. En este caso, se está incluyendo el correo electrónico del usuario.

```csharp
var claims = new List<Claim>() { new(ClaimTypes.Name, request.Email), };
```

4. Luego, se crea una clave de seguridad utilizando la clave secreta proporcionada. Esta clave se utilizará para firmar el token.

```csharp
var securityKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                                "aopsjfp0aoisjf[poajsf[poajsp[fojasp[foja[psojf[paosjfp[aojsfpaojsfp[ojasf"));
var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
```

5. A continuación, se crea el token JWT utilizando el emisor, el público, las reclamaciones, la fecha de vencimiento y las credenciales de firma.

```csharp
var jwtSecurityToken = new JwtSecurityToken(
                        issuer: "https://www.surymartinez.com",
                        audience: "Minimal APIs Client",
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(1),
                        signingCredentials: credentials);
```

6. Finalmente, se escribe el token JWT en una cadena y se devuelve como parte de la respuesta.

```csharp
var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

return Results.Ok(new { AccessToken = accessToken });
```

Si la verificación del usuario falla, se devuelve un error 400 (BadRequest).

```csharp
return Results.BadRequest();
```

___
### Configuración de JWT en la API
La declaración de servicios para la generación de JWT en el archivo `Program.cs` se realiza en varias partes:

1. Primero, se configura el servicio de autenticación para usar JWT (JSON Web Tokens) como el esquema de autenticación predeterminado. Esto se hace utilizando `AddAuthentication(JwtBearerDefaults.AuthenticationScheme)`.

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
```

2. Luego, se configura el servicio JWT Bearer. Aquí es donde se establecen los parámetros de validación del token. Se valida el emisor del token (`ValidateIssuer = true`), se establece la clave de firma del token (`IssuerSigningKey`) y se definen el emisor válido (`ValidIssuer`) y la audiencia válida (`ValidAudience`).

```csharp
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aopsjfp0aoisjf[poajsf[poajsp[fojasp[foja[psojf[paosjfp[aojsfpaojsfp[ojasf")),
        ValidIssuer = "https://www.surymartinez.com", 
        ValidAudience = "Minimal APIs Client" 
    };
})
```

3. Después de configurar la autenticación, se agrega el servicio de autorización con `AddAuthorization()`.

```csharp
builder.Services.AddAuthorization();
```

4. Finalmente, se configura Swagger para usar JWT. Se agrega una definición de seguridad para JWT y se agrega un requisito de seguridad que hace referencia a esa definición.

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
        new OpenApiSecurityScheme
        { 
            Type = SecuritySchemeType.ApiKey, 
            In = ParameterLocation.Header, 
            Name = HeaderNames.Authorization, 
            Description = "Insert the token with the 'Bearer ' prefix", 
        });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { 
            new OpenApiSecurityScheme 
            { 
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id = JwtBearerDefaults.AuthenticationScheme 
                } 
            },
            new string[] { } 
        } 
    });
});
```

Estas configuraciones permiten que la aplicación genere y valide JWT para la autenticación de usuarios.


## Users

### Declaracion de la clase o record User
El `UserDto` es un `record` en C#, introducido en C# 9.0. Un `record` es un tipo de referencia que tiene características de inmutabilidad y comportamiento de valor. Esto significa que una vez que un `record` es creado, no puede ser modificado. Cualquier modificación resultará en la creación de un nuevo `record`.

Aquí está el `UserDto`:

```csharp
public record UserDto(string Email, string Password);
```

Este `record` tiene dos propiedades, `Email` y `Password`, que son inmutables. Esto significa que una vez que se crea una instancia de `UserDto`, no puedes cambiar el `Email` o `Password`.

La principal diferencia entre un `record` y una clase es que un `record` es inmutable y tiene comportamiento de valor. Esto significa que dos `records` con los mismos valores serán considerados iguales. En contraste, dos instancias de una clase con los mismos valores no son iguales a menos que se sobrescriba el método `Equals`.

Además, los `records` proporcionan funcionalidades incorporadas para copiar y comparar objetos. Por ejemplo, puedes crear una copia de un `record` con algunas propiedades modificadas utilizando la sintaxis `with`.

En resumen, debes usar `records` cuando quieras modelos inmutables con comportamiento de valor, y clases cuando necesites objetos con estado mutable.

___
### Servicio de usuarios
El archivo `UserService.cs` contiene una clase llamada `UserService` que se utiliza para manejar las operaciones relacionadas con los usuarios en la aplicación. Aquí está un desglose de su funcionamiento:

1. La clase `UserService` se inicializa con una lista de `UserDto`. Esta lista se utiliza como una base de datos en memoria para almacenar los usuarios.

```csharp
public class UserService(List<UserDto> users)
{
    private readonly List<UserDto> _users = users;
}
```

2. La función `CreateUser` toma un `UserDto` como argumento, lo agrega a la lista de usuarios y luego lo devuelve.

```csharp
public UserDto CreateUser(UserDto newUser)
{
    _users.Add(newUser);
    return newUser;
}
```

3. La función `GetAllUsers` devuelve todos los usuarios en la lista.

```csharp
public List<UserDto> GetAllUsers()
{
    return _users;
}
```

4. La función `GetUser` toma un `userId` como argumento, busca un usuario con ese ID en la lista y lo devuelve. Si no se encuentra ningún usuario, devuelve `null`.

```csharp
public UserDto? GetUser(string userId)
{
    return _users.Find(u => u.Email == userId);
}
```

5. La función `UpdateUserPassword` toma un `userId` y un `UserDto` como argumentos. Busca un usuario con ese ID en la lista y, si lo encuentra, actualiza su contraseña con la del `UserDto` proporcionado y lo devuelve. Si no se encuentra ningún usuario, devuelve `null`.

```csharp
public UserDto? UpdateUserPassword(string userId, UserDto updatedUser)
{
    var user = _users.Find(u => u.Email == userId);
    if (user != null)
    {
        user = updatedUser;
        return user;
    }
    return null;
}
```

En resumen, `UserService` es una clase simple que proporciona funcionalidades para crear, obtener y actualizar usuarios en una base de datos en memoria.

___
### Endpoints de usuarios
Los endpoints `/users` en el archivo `Program.cs` interactúan con la clase `UserService` para realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) en los usuarios.

1. `app.MapPost("/users", (UserDto newUser) => {...})`: Este es un endpoint POST que se utiliza para crear un nuevo usuario. Toma un `UserDto` como cuerpo de la solicitud, lo pasa al método `CreateUser` de `UserService`, y luego devuelve el usuario creado. Si la creación es exitosa, devuelve un estado 201 (Created) con la ubicación del nuevo recurso y el recurso creado.

```csharp
app.MapPost("/users", (UserDto newUser) =>
{
    var createdUser = userService.CreateUser(newUser);
    return Results.Created($"/users/{createdUser.Email}", createdUser);
}).WithName("CreateUser").WithOpenApi();
```

2. `app.MapGet("/users", () => {...})`: Este es un endpoint GET que se utiliza para obtener todos los usuarios. Llama al método `GetAllUsers` de `UserService` y devuelve la lista de todos los usuarios.

```csharp
app.MapGet("/users", () => userService.GetAllUsers()).WithName("GetAllUsers").WithOpenApi();
```

3. `app.MapGet("/users/{userId}", (string userId) => {...})`: Este es un endpoint GET que se utiliza para obtener un usuario específico por su ID (en este caso, el correo electrónico). Toma un `userId` como parámetro de ruta, lo pasa al método `GetUser` de `UserService`, y luego devuelve el usuario si se encuentra. Si no se encuentra el usuario, devuelve un estado 404 (Not Found).

```csharp
app.MapGet("/users/{userId}", (string userId) =>
{
    var user = userService.GetUser(userId);
    if (user == null)
    {
        return Results.NotFound($"User with ID {userId} not found.");
    }
    return Results.Ok(user);
});
```

4. `app.MapPut("/users/{userId}", (string userId, UserDto updatedUser) => {...})`: Este es un endpoint PUT que se utiliza para actualizar la contraseña de un usuario específico. Toma un `userId` como parámetro de ruta y un `UserDto` como cuerpo de la solicitud, los pasa al método `UpdateUserPassword` de `UserService`, y luego devuelve el usuario actualizado si se encuentra. Si no se encuentra el usuario, devuelve un estado 404 (Not Found).

```csharp
app.MapPut("/users/{userId}", (string userId, UserDto updatedUser) =>
{
    var user = userService.UpdateUserPassword(userId, updatedUser);
    if (user == null)
    {
        return Results.NotFound($"User with ID {userId} not found.");
    }
    return Results.Ok(user);
}).WithName("UpdateUserPassword").WithOpenApi();
```

En resumen, estos endpoints proporcionan una interfaz HTTP para interactuar con la clase `UserService`, permitiendo a los clientes de la API realizar operaciones CRUD en los usuarios.

___
### Funcionamiento de los endpoints de la API
~~~mermaid
sequenceDiagram
    participant Cliente
    participant Login as API: /auth/login
    participant Users as API: /users
    participant UserService

    Cliente->>Login: POST (email, password)
    Login->>UserService: GetUser(email)
    UserService-->>Login: UserDto (email, password)
    Login->>Login: Verificar contraseña
    Login->>Login: Generar JWT
    Login-->>Cliente: JWT

    Cliente->>Users: POST (UserDto)
    Users->>UserService: CreateUser(UserDto)
    UserService-->>Users: UserDto (email, password)
    Users-->>Cliente: UserDto (email, password)

    Cliente->>Users: GET
    Users->>UserService: GetAllUsers()
    UserService-->>Users: List<UserDto>
    Users-->>Cliente: List<UserDto>

    Cliente->>Users: GET (userId)
    Users->>UserService: GetUser(userId)
    UserService-->>Users: UserDto (email, password)
    Users-->>Cliente: UserDto (email, password)

    Cliente->>Users: PUT (userId, UserDto)
    Users->>UserService: UpdateUserPassword(userId, UserDto)
    UserService-->>Users: UserDto (email, password)
    Users-->>Cliente: UserDto (email, password)
~~~
___
### Diagrama de clases API
~~~mermaid
classDiagram
    WebApplication --|> WebApplicationBuilder: Creates
    WebApplicationBuilder --> UserService: Adds to services
    WebApplication --> UserService: Uses
    UserService --> UserDto: Manages
    class WebApplication {
        +CreateBuilder(args: string[]): WebApplicationBuilder
        +Build(): WebApplication
    }
    class WebApplicationBuilder {
        +Services: IServiceCollection
        +Build(): WebApplication
    }
    class WebApplication {
        +Services: IServiceProvider
        +UseSwagger(): WebApplication
        +UseSwaggerUI(): WebApplication
        +UseHttpsRedirection(): WebApplication
        +UseAuthentication(): WebApplication
        +UseAuthorization(): WebApplication
        +MapGet(pattern: string, handler: Delegate): WebApplication
        +MapPost(pattern: string, handler: Delegate): WebApplication
        +MapPut(pattern: string, handler: Delegate): WebApplication
        +Run(): void
    }
    class UserService {
        +CreateUser(newUser: UserDto): UserDto
        +GetAllUsers(): List<UserDto>
        +GetUser(userId: string): UserDto?
        +UpdateUserPassword(userId: string, updatedUser: UserDto): UserDto?
    }
    class UserDto {
        +Email: string
        +Password: string
    }
~~~
