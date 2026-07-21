using SistemGradum.Application.DTOs;

namespace SistemGradum.Application.Interfaces;

public interface IAuthService
{
    LoginResponseDto? IniciarSesion(LoginRequestDto dto);
}