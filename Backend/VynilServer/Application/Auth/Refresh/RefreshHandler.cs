using MediatR;
using Microsoft.EntityFrameworkCore;
using VynilServer.Application.Common;
using VynilServer.Data;
using VynilServer.Infrastructure.Security.JwtTokenService;

namespace VynilServer.Application.Auth.Refresh;

public class RefreshHandler(
    AppDbContext dbContext,
    ITokenService tokenService
) : IRequestHandler<RefreshCommand, Result<(string RefreshToken, string AccessToken)>>
{
    public async Task<Result<(string RefreshToken, string AccessToken)>> Handle(RefreshCommand request, CancellationToken cnt)
    {
        if (!tokenService.ValidateRefreshToken(request.RefreshToken, request.Fingerprint))
            return Result<(string, string)>.Failure("Невалидный токен");

        var tokenData = tokenService.DecodeRefreshToken(request.RefreshToken);
        if (tokenData == null)
            return Result<(string, string)>.Failure("Не удалось извлечь данные токена");

        var user = await dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == tokenData.UserId, cnt);

        if (user == null)
            return Result<(string, string)>.Failure("Пользователь не найден");

        if (!user.RefreshTokens.Contains(request.RefreshToken))
            return Result<(string, string)>.Failure("Токен отозван");

        user.RefreshTokens.Remove(request.RefreshToken);

        var newRefreshToken = tokenService.GenerateRefreshToken(user.Id, request.Fingerprint);
        var newAccessToken = tokenService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());

        user.RefreshTokens.Add(newRefreshToken);
        await dbContext.SaveChangesAsync(cnt);

        return Result<(string, string)>.Success((newRefreshToken, newAccessToken));
    }
}