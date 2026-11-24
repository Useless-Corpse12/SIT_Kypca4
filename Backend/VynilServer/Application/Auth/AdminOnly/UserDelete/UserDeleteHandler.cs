using MediatR;
using Microsoft.EntityFrameworkCore;
using VynilServer.Application.Common;
using VynilServer.Data;

namespace VynilServer.Application.Auth;

public class UserDeleteHandler(
    AppDbContext dbContext
    ):IRequestHandler<UserDeleteCommand,Result<bool>>
{
    public async Task<Result<bool>> Handle(UserDeleteCommand request, CancellationToken cnt)
    {
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(x=>x.Id==request.UserId,cnt);
        if (existingUser == null)
            return Result<bool>.Failure("Не нашли цель для бана");

        dbContext.Users.Remove(existingUser);
        await dbContext.SaveChangesAsync(cnt);
        return Result<bool>.Success(true);
    }
}