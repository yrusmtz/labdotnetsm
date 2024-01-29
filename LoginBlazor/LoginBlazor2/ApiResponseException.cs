using LoginShared.Security.DTOs;

namespace LoginBlazor2;

public class ApiResponseException : Exception
{
    public ApiErrorResponseDto ErrorDetails { get; set; }
    public ApiResponseException(ApiErrorResponseDto errorDetails) : base(errorDetails.Message)
    {
        ErrorDetails = errorDetails;
    }
}
