using MediatR;
using Microsoft.EntityFrameworkCore;
using VynilServer.Application.Common;
using VynilServer.Data;
using VynilServer.Domain.Entities;
using VynilServer.Infrastructure.Security.Hasher;
using VynilServer.Infrastructure.Security.JwtTokenService;
using VynilServer.Infrastructure.Services.CodeService;
using VynilServer.Infrastructure.Services.EmailService;

namespace VynilServer.Application.Auth;

public class LoginHandler(
    AppDbContext dbContext,
    IEmailSender emailSender,
    ICodeService codeService,
    ITokenService tokenService,
    IPasswordHasher passwordHasher
    ):IRequestHandler<LoginCommand,Result<string>>
{
    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cnt)
    {
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(x=>x.Email == request.Email,cnt);
        
        if (existingUser == null) 
            return Result<string>.Failure("Неверный логин или пароль");
        if (!existingUser.IsVerified)
        {
            var code = codeService.GenerateAndSaveCode(existingUser.Id, CodeType.VerificationCode);
            await emailSender.SendDynamicEmailAsync(
                toEmail:existingUser.Email,
                subject:"Код подтверждения",
                labels: new []{"Код подтверждения"},
                values: new []{code});
            return  Result<string>.Failure(null);
        }

        if (!passwordHasher.Validate(request.Password, existingUser.PasswordHash))
            return Result<string>.Failure("Неверный логин или пароль");

        if (existingUser.Role == Role.Admin)
        {
            var code = codeService.GenerateAndSaveCode(existingUser.Id, CodeType.LoginCode);
            await emailSender.SendDynamicEmailAsync(
                toEmail:existingUser.Email,
                subject:"Подтверждение входа",
                labels:new [] {"Код для подтверждения входа"},
                values:new [] {code});
            return Result<string>.Success(null);
        }

        var token = tokenService.GenerateRefreshToken(existingUser.Id, request.Fingerprint);
        
        existingUser.RefreshTokens.Add(token);
        await dbContext.SaveChangesAsync(cnt);
        
        return Result<string>.Success(token);
    }
}