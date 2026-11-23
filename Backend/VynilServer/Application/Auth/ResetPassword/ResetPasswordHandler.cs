using MediatR;
using Microsoft.EntityFrameworkCore;
using VynilServer.Application.Common;
using VynilServer.Data;
using VynilServer.Infrastructure.Security.Hasher;
using VynilServer.Infrastructure.Services.CodeService;

namespace VynilServer.Application.Auth.ResetPassword;

public class ResetPasswordHandler(
    AppDbContext dbContext,
    ICodeService codeService,
    IPasswordHasher passwordHasher
    ):IRequestHandler<ResetPasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cnt)
    {
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.Id, cnt);
        if (existingUser == null)
            return Result<bool>.Failure("Такого пользователя не существует");
        
        if (!codeService.ValidateCode(existingUser.Id,request.Code,CodeType.PasswordCode))
            return Result<bool>.Success(false);

        existingUser.PasswordHash = passwordHasher.Create(request.NewPassword);
        existingUser.RefreshTokens.Clear();
        
        await  dbContext.SaveChangesAsync(cnt);
        return Result<bool>.Success(true);
    }
}