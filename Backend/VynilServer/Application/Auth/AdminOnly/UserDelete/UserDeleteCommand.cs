using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth;

public record UserDeleteCommand(Guid UserId):IRequest<Result<bool>>;