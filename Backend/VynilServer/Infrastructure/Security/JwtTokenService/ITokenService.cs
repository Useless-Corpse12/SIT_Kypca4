namespace VynilServer.Infrastructure.Security.JwtTokenService;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId,string email,string role);
    string GenerateRefreshToken(Guid userId,string fingerprint);
    bool ValidateRefreshToken(string refreshToken,string fingerprint);
}