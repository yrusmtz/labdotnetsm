using LoginShared.Security.DTOs;
using System.Net.Http.Json;

namespace LoginBlazor2.Security.Authentication;

public class AuthenticationService : IAuthenticationService
{

    // Inject the HttpClient into the constructor
    private readonly HttpClient _httpClient;

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponseDto> LoginUserAsync(LoginRequestDto requestModel)
    {
        var response = await _httpClient.PostAsJsonAsync("authentication/login", requestModel);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            // Handle the bad request as the API doc says
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
            throw new ApiResponseException(error);
        }
        else
        {
            // Throw exception for other failure responses 
            throw new Exception("Opps! Something went wrong");
        }
    }
}
