using SistemGradum.Application.DTOs;

namespace SistemGradum.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
}