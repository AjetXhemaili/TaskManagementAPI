using TaskManagementAPI.DTOs;

namespace TaskManagementAPI.Services
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(RegisterDto dto);
        Task<UserResponseDto> LoginAsync(LoginDto dto);
    }
}
