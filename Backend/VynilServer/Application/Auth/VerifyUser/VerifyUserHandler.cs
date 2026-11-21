using MediatR;
using Microsoft.EntityFrameworkCore;
using VynilServer.Application.Common;
using VynilServer.Data;
using VynilServer.Infrastructure.Services.CodeService;

namespace VynilServer.Application.Auth.VerifyUser;

public class VerifyUserHandler(ICodeService codeService,AppDbContext dbContext):IRequestHandler<VerifyUserCommand,Result<bool>>
{
    async public Task<Result<bool>> Handle(VerifyUserCommand request, CancellationToken cnt)
    {
        if (codeService.ValidateCode(request.UserId, request.Code, CodeType.VerificationCode))
        {
            var verifiedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cnt);
            if (verifiedUser==null) return Result<bool>.Failure("Пользователь не найден"); 
            verifiedUser.VerifyEmail();
            await dbContext.SaveChangesAsync(cnt);
            return Result<bool>.Success(true);
        }
        
        return Result<bool>.Success(false);
    }
}