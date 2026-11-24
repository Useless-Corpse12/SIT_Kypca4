using MediatR;
using Microsoft.EntityFrameworkCore;
using VynilServer.Application.Common;
using VynilServer.Data;
using VynilServer.Domain.Entities;
using VynilServer.Infrastructure.Security.Hasher;
using VynilServer.Infrastructure.Services.EmailService;

namespace VynilServer.Application.Auth;

public class AdminCreateHandler(
    AppDbContext dbContext,
    IEmailSender emailSender,
    IPasswordHasher passwordHasher
    ):IRequestHandler<AdminCreateCommand,Result<bool>>
{
    public async Task<Result<bool>> Handle(AdminCreateCommand request, CancellationToken cnt)
    {
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(x=>x.Email == request.Email,cnt);
        if (existingUser != null)
            return Result<bool>.Failure("Пользователь существует, либо ошибка в почте");
        
        var randomPass = passwordHasher.Create(Guid.NewGuid().ToString());
        var randomLogin = $"Admin{Guid.NewGuid().ToString()}";
        var newUser = User.CreateAdmin(
            login: randomLogin, 
            email: request.Email,
            passwordHash: randomPass);
        
        await dbContext.Users.AddAsync(newUser,cnt);

        var regLink = "http://registration-page";

        await emailSender.SendDynamicEmailAsync(
            toEmail: request.Email,
            subject: "Приглашение в администрацию",
            labels: new[] { "Для приглашения пройдите процедуру регистрации" },
            values: new[] { regLink });

        await dbContext.SaveChangesAsync(cnt);
        return Result<bool>.Success(true);
    }
}