using MediatR;
using Microsoft.EntityFrameworkCore;
using VynilServer.Application.Common;
using VynilServer.Data;
using VynilServer.Infrastructure.Services.CodeService;
using VynilServer.Infrastructure.Services.EmailService;

namespace VynilServer.Application.Auth;

public class ResendCodeHandler(
    AppDbContext dbContext,
    IEmailSender emailSender,
    ICodeService codeService
    ):IRequestHandler<ResendCodeCommand,Result<bool>>
{
    public async Task<Result<bool>> Handle(ResendCodeCommand request, CancellationToken cnt)
    {
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cnt);
        if (existingUser == null)
            return Result<bool>.Failure("При отправке сообщения произошла ошибка");
        if (existingUser.IsVerified)
            return Result<bool>.Success(false);
        var code = codeService.GenerateAndSaveCode(existingUser.Id, CodeType.VerificationCode);
        
        await emailSender.SendDynamicEmailAsync(
            toEmail:existingUser.Email,
            subject:"Код подтверждения",
            labels:new[] {"Код подтверждения"},
            values:new[] {code});
        
        return Result<bool>.Success(true);
    }
}