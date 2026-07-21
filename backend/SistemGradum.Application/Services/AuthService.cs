using SistemGradum.Application.DTOs;
using SistemGradum.Application.Interfaces;

namespace SistemGradum.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository usuarioRepository;
    private readonly ITokenService tokenService;
    private readonly IAlertaService alertaService;

    public AuthService(
        IUsuarioRepository usuarioRepository, ITokenService tokenService, IAlertaService alertaService)
    {
        this.usuarioRepository = usuarioRepository;
        this.tokenService = tokenService;
        this.alertaService = alertaService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var usuario = await this.usuarioRepository.GetByUsernameAsync(request.NombreUsuario);

        if (usuario is null)
            return null;

        bool passwordValida = BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash);

        if (!passwordValida)
            return null;

        var token = this.tokenService.GenerarToken(usuario);

        // RF-022: genera alertas al iniciar sesión (no como proceso en segundo plano).
        var asesorIdFiltro = usuario.Rol == "Asesor" ? usuario.AsesorId : null;
        await this.alertaService.GenerarAlertasAsync(usuario.Id, usuario.Rol, asesorIdFiltro);

        return new LoginResponseDto
        {
            Token = token,
            NombreUsuario = usuario.NombreUsuario,
            Rol = usuario.Rol,
            AsesorId = usuario.AsesorId,
            ExpiraEn = DateTime.UtcNow.AddMinutes(60)
        };
    }
}