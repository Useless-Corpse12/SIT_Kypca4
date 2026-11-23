using MediatR;
using Microsoft.EntityFrameworkCore;
using VynilServer.Application.Common;
using VynilServer.Data;
using VynilServer.Infrastructure.Security.Hasher;

namespace VynilServer.Application.Auth;

public class ChangePasswordHandler(
    AppDbContext dbContext,
    IPasswordHasher passwordHasher
    ):IRequestHandler<ChangePasswordCommand,Result<bool>>
{
    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cnt)
    {
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(x=>x.Id == request.UserId,cnt);

        if (existingUser == null)
            return Result<bool>.Failure("Мы вас потеряли...");

        if (!passwordHasher.Validate(request.OldPassword, existingUser.PasswordHash))
            return Result<bool>.Failure("Неверный пароль");

        existingUser.PasswordHash = passwordHasher.Create(request.NewPassword);
        
        await dbContext.SaveChangesAsync(cnt);
        return Result<bool>.Success(true);
    }
}