using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LoginBlazor2;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _storage;
    public JwtAuthenticationStateProvider(ILocalStorageService storage)
    {
        _storage = storage;
    }

    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (await _storage.ContainKeyAsync("access_token"))
        {
            // Read and parse the token 
            string? tokenAsString = await _storage.GetItemAsync<string>("access_token");
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenAsString is null)
            {
                return returnAnonymousAuthState();
            }
            JwtSecurityToken token = tokenHandler.ReadJwtToken(tokenAsString);

            var identity = new ClaimsIdentity(token.Claims, "Bearer");
            var user = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(authState));

            return authState;
        }

        return returnAnonymousAuthState();

    }

    private AuthenticationState returnAnonymousAuthState()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity()); // No claims or authentication scheme provided
        var anonymousAuthState = new AuthenticationState(anonymousUser);
        NotifyAuthenticationStateChanged(Task.FromResult(anonymousAuthState));
        return anonymousAuthState;
    }
}
