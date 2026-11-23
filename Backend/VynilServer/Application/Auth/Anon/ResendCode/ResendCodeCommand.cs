using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth;

public record ResendCodeCommand(Guid UserId) : IRequest<Result<bool>>;