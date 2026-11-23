using MediatR;
using Microsoft.EntityFrameworkCore;
using VynilServer.Application.Common;
using VynilServer.Data;
using VynilServer.Domain.Entities;
using VynilServer.Infrastructure.Security.Hasher;
using VynilServer.Infrastructure.Services.EmailService;
using VynilServer.Infrastructure.Services.CodeService;

namespace VynilServer.Application.Auth;

public class RegisterUserHandler(
    AppDbContext dbContext, 
    IPasswordHasher passwordHasher, 
    IEmailSender emailSender,
    ICodeService codeService)
:IRequestHandler<RegisterUserCommand,Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cnt)
    {
        
        var emailLower = request.Email.ToLower();
        
        var existingUser = await dbContext.Users.
            FirstOrDefaultAsync(x => x.Email == emailLower, cnt);

        if (existingUser != null)
        {
            if (existingUser.IsVerified)
            { return Result<Guid>.Failure("Пользователь с такой почтой уже зарегистрирован"); }
            
            existingUser.Login = request.Login;
            existingUser.PasswordHash = passwordHasher.Create(request.Password);
            
            codeService.RemoveCode(existingUser.Id,CodeType.VerificationCode);
            var code = codeService.GenerateAndSaveCode(existingUser.Id,CodeType.VerificationCode);
            
            await dbContext.SaveChangesAsync(cnt);
            
            await emailSender.SendDynamicEmailAsync(
                toEmail:existingUser.Email,
                subject:"Код подтверждения",
                labels:new[] {"Код подтверждения"},
                values:new[] {code});
            
            Console.WriteLine($"Обновлён пользователь. Новый код: {code}");
            
            return Result<Guid>.Success(existingUser.Id);
        }
        
        var user = User.CreateUser(request.Login,emailLower, passwordHasher.Create(request.Password));
        
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cnt);
        
        var newCode = codeService.GenerateAndSaveCode(user.Id,CodeType.VerificationCode);
        
        await emailSender.SendDynamicEmailAsync(
            toEmail:user.Email,
            subject:"Код подтверждения",
            labels:new[] {"Код подтверждения"},
            values:new[] {newCode});
        
        Console.WriteLine($"Создан пользователь. Код: {newCode}");
        
        return Result<Guid>.Success(user.Id);
    }
}