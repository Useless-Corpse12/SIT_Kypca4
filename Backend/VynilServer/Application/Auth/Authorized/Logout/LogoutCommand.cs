using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth;

public record LogoutCommand(Guid? UserId, string RefreshToken):IRequest<Result<bool>>;