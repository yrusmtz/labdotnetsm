using LoginShared;
using LoginShared.Security.DTOs;

namespace LoginBlazor2.Security.Authentication;

public interface IAuthenticationService
{
    Task<LoginResponseDto> LoginUserAsync(LoginRequestDto requestModel);
}
