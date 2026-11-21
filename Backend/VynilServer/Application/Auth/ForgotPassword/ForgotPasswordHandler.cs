using MediatR;
using Microsoft.EntityFrameworkCore;
using VynilServer.Application.Common;
using VynilServer.Data;
using VynilServer.Infrastructure.Services.CodeService;
using VynilServer.Infrastructure.Services.EmailService;

namespace VynilServer.Application.Auth.ForgotPassword;

public class ForgotPasswordHandler(
    AppDbContext dbContext,
    ICodeService codeService,
    IEmailSender emailSender):IRequestHandler<ForgotPasswordCommand,Result<bool>>
{
    public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken cnt)
    {
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email,cnt);
        if (existingUser == null)
            return Result<bool>.Failure("Произошла ошибка, проверьте правильность введения данных");

        if (!existingUser.IsVerified)
            return Result<bool>.Success(false);

        var newCode = codeService.GenerateAndSaveCode(existingUser.Id, CodeType.PasswordCode);
        var resetPasswordLink = "???";

        await emailSender.SendDynamicEmailAsync(
            toEmail: existingUser.Email,
            subject: "Восстановление пароля",
            labels: new[] { "Перейдите по ссылке для сброса пароля", "или введите код" },
            values: new[] { resetPasswordLink, newCode });
        
        return Result<bool>.Success(true);
    }
}