namespace VynilServer.Infrastructure.Security.JwtTokenService;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId,string email,string role);
    string GenerateRefreshToken(Guid userId,string fingerprint);
    bool ValidateRefreshToken(string refreshToken,string fingerprint);
    RTokenData? DecodeRefreshToken(string refreshToken);
    ATokenData? DecodeAccessToken(string accessToken);
}

public record ATokenData(Guid UserId, string Email, string Role);
public record RTokenData(Guid UserId);