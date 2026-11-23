using MediatR;
using Microsoft.EntityFrameworkCore;
using VynilServer.Application.Common;
using VynilServer.Data;
using VynilServer.Infrastructure.Security.Hasher;

namespace VynilServer.Application.Auth;

public class SelfDeleteHandler(
    AppDbContext dbContext,
    IPasswordHasher passwordHasher
    ):IRequestHandler<SelfDeleteCommand,Result<bool>>
{
    public async Task<Result<bool>> Handle(SelfDeleteCommand request, CancellationToken cnt)
    {
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(x=>x.Id==request.UserId,cnt);

        if (existingUser == null)
            return Result<bool>.Failure("Мы вас потеряли...");

        if (!passwordHasher.Validate(request.Password, existingUser.PasswordHash))
            return Result<bool>.Failure("Неверный пароль");
        
        existingUser.Anonymize();
        await dbContext.SaveChangesAsync(cnt);
        return Result<bool>.Success(true);
    }
}