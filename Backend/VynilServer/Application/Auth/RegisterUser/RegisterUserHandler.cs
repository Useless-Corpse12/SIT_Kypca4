using MediatR;
using Microsoft.EntityFrameworkCore;
using VynilServer.Application.Common;
using VynilServer.Data;
using VynilServer.Domain.Entities;
using VynilServer.Infrastructure.Security.Hasher;
using VynilServer.Infrastructure.Services.EmailService;
using VynilServer.Infrastructure.Services.VerificationCode;

namespace VynilServer.Application.Auth.RegisterUser;

public class RegisterUserHandler(
    AppDbContext dbContext, 
    IPasswordHasher passwordHasher, 
    IEmailSender emailSender,
    IVerificationCodeService codeService)
:IRequestHandler<RegisterUserCommand,Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cnt)
    {
        
        var emailLower = request.email.ToLower();
        
        var existingUser = await dbContext.Users.
            FirstOrDefaultAsync(x => x.Email == emailLower, cnt);

        if (existingUser != null)
        {
            if (existingUser.IsVerified)
            { return Result<Guid>.Failure("Пользователь с такой почтой уже зарегистрирован"); }
            
            existingUser.Login = request.login;
            existingUser.PasswordHash = passwordHasher.Create(request.password);
            
            codeService.RemoveCode(existingUser.Id);
            var code = codeService.GenerateAndSaveCode(existingUser.Id);
            
            dbContext.Users.Update(existingUser);
            await dbContext.SaveChangesAsync(cnt);
            
            await emailSender.SendRegistrationCodeAsync(existingUser.Email, code);
            Console.WriteLine($"Обновлён пользователь. Новый код: {code}");
            
            return Result<Guid>.Success(existingUser.Id);
        }
        
        var user = User.CreateUser(request.login,emailLower, passwordHasher.Create(request.password));
        
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cnt);
        
        var newCode = codeService.GenerateAndSaveCode(user.Id);
        
        await emailSender.SendRegistrationCodeAsync(user.Email, newCode);
        Console.WriteLine($"Создан пользователь. Код: {newCode}");
        
        return Result<Guid>.Success(user.Id);
    }
}