using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth;

public record TotalLogoutCommand(Guid? UserId, string RefreshToken):IRequest<Result<bool>>;