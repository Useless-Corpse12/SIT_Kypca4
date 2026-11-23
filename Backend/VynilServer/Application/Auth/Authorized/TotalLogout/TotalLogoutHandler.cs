using MediatR;
using Microsoft.EntityFrameworkCore;
using VynilServer.Application.Common;
using VynilServer.Data;
using VynilServer.Infrastructure.Security.JwtTokenService;

namespace VynilServer.Application.Auth;

public class TotalLogoutHandler(
    AppDbContext dbContext,
    ITokenService tokenService
    ):IRequestHandler<TotalLogoutCommand,Result<bool>>
{
    public async Task<Result<bool>> Handle(TotalLogoutCommand request, CancellationToken cnt)
    {
        var tokenDecoded = tokenService.DecodeRefreshToken(request.RefreshToken);
        
        if (tokenDecoded == null)
            return Result<bool>.Failure("Токен засран");

        if (tokenDecoded.UserId != request.UserId)
            return Result<bool>.Failure("Ты кто?");
        
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(x=>x.Id == request.UserId, cnt);

        if (existingUser == null)
            return Result<bool>.Failure("Ты кто?");

        if (!existingUser.RefreshTokens.Contains(request.RefreshToken))
            return Result<bool>.Failure("Токен засран");

        existingUser.RefreshTokens.Clear();
        await dbContext.SaveChangesAsync(cnt);
        
        return Result<bool>.Success(true);
    }
}