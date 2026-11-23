using MediatR;
using Microsoft.EntityFrameworkCore;
using VynilServer.Data;
using VynilServer.Infrastructure.Security.JwtTokenService;
using VynilServer.Infrastructure.Services.CodeService;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth.VerifyLogin;

public class VerifyLoginHandler(
    AppDbContext dbContext,
    ICodeService codeService,
    ITokenService tokenService
    ):IRequestHandler<VerifyLoginCommand,Result<(string RefreshToken,string AccessToken)>>
{
    public async Task<Result<(string RefreshToken, string AccessToken)>> Handle(VerifyLoginCommand request,
        CancellationToken cnt)
    {
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cnt);
        
        if (existingUser == null)
            return Result<(string, string)>.Failure("Ой, мы прошляпили твой акк");

        if (!codeService.ValidateCode(existingUser.Id, request.Code, CodeType.LoginCode))
            return Result<(string, string)>.Failure("Код не подошёл, попробуй ещё раз");
        
        var rfrToken = tokenService.GenerateRefreshToken(existingUser.Id,request.Fingerprint);
        var acsToken =
            tokenService.GenerateAccessToken(existingUser.Id, existingUser.Email, existingUser.Role.ToString());
        
        existingUser.RefreshTokens.Add(rfrToken);
        
        await dbContext.SaveChangesAsync(cnt);
        
        return Result<(string,string)>.Success((rfrToken,acsToken)); 
    }
}